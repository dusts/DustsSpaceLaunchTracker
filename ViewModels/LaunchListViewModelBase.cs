using CommunityToolkit.Mvvm.Input;
using DustsSpaceLaunchTracker.Configuration;
using DustsSpaceLaunchTracker.Models.Responses;
using DustsSpaceLaunchTracker.Services;
using DustsSpaceLaunchTracker.Services.Diagnostics;
using System.Collections.ObjectModel;
using System.Net;

namespace DustsSpaceLaunchTracker.ViewModels
{
    /// <summary>
    /// Shared pagination, search, cancel, countdown tick for launch lists (points 3, 11, 13).
    /// </summary>
    public abstract partial class LaunchListViewModelBase : ViewModelBase, IDisposable
    {
        private readonly ILaunchService _launchService;
        private readonly IDiagnosticsService _diagnostics;
        private readonly Func<ILaunchService, int, int, string?, int?, int?, bool, CancellationToken, Task<PagedResult<Models.Launch>>> _pageFetcher;
        private readonly string _title;

        private CancellationTokenSource? _loadCts;
        private IDispatcherTimer? _countdownTimer;
        private DateTime _lastLoadMoreUtc = DateTime.MinValue;
        private int _offset;
        private bool _initialized;
        private bool _disposed;

        private bool _isBusy;
        private bool _isRefreshing;
        private bool _isLoadingMore;
        private string? _errorMessage;
        private string? _errorDetail;
        private bool _showErrorDetail;
        private bool _hasMoreItems = true;
        private int _totalCount;
        private string _statusText = string.Empty;
        private bool _showEndOfList;
        private string _searchText = string.Empty;
        private string? _activeSearch;
        private StatusFilterOption _selectedStatusFilter = StatusFilterOption.All;
        private int _selectedStatusIndex;
        private bool _isFromCacheNotice;
        private bool _suppressStatusFilterReload;

        protected LaunchListViewModelBase(
            ILaunchService launchService,
            IDiagnosticsService diagnostics,
            string title,
            Func<ILaunchService, int, int, string?, int?, int?, bool, CancellationToken, Task<PagedResult<Models.Launch>>> pageFetcher)
        {
            _launchService = launchService;
            _diagnostics = diagnostics;
            _title = title;
            _pageFetcher = pageFetcher;
        }

        public string Title => _title;

        public ObservableCollection<LaunchListItemViewModel> Launches { get; } = new();

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                    LoadMoreLaunchesCommand.NotifyCanExecuteChanged();
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            private set => SetProperty(ref _isRefreshing, value);
        }

        public bool IsLoadingMore
        {
            get => _isLoadingMore;
            private set => SetProperty(ref _isLoadingMore, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set
            {
                if (SetProperty(ref _errorMessage, value))
                    OnPropertyChanged(nameof(HasError));
            }
        }

        /// <summary>Full exception text for expand/copy on mobile.</summary>
        public string? ErrorDetail
        {
            get => _errorDetail;
            private set
            {
                if (SetProperty(ref _errorDetail, value))
                    OnPropertyChanged(nameof(HasErrorDetail));
            }
        }

        public bool ShowErrorDetail
        {
            get => _showErrorDetail;
            set => SetProperty(ref _showErrorDetail, value);
        }

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public bool HasErrorDetail => !string.IsNullOrWhiteSpace(ErrorDetail);

        public bool HasMoreItems
        {
            get => _hasMoreItems;
            private set
            {
                if (SetProperty(ref _hasMoreItems, value))
                    LoadMoreLaunchesCommand.NotifyCanExecuteChanged();
            }
        }

        public int TotalCount
        {
            get => _totalCount;
            private set => SetProperty(ref _totalCount, value);
        }

        public string StatusText
        {
            get => _statusText;
            private set => SetProperty(ref _statusText, value);
        }

        public bool ShowEndOfList
        {
            get => _showEndOfList;
            private set => SetProperty(ref _showEndOfList, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool IsFromCacheNotice
        {
            get => _isFromCacheNotice;
            private set => SetProperty(ref _isFromCacheNotice, value);
        }

        /// <summary>Status filter options for the picker (point 11).</summary>
        public IReadOnlyList<StatusFilterOption> StatusFilters { get; } = StatusFilterOption.AllOptions;

        /// <summary>
        /// Prefer index binding for MAUI Picker — SelectedItem often becomes null for "All"
        /// and was causing NullReferenceException on Previous (and any) list load.
        /// </summary>
        public int SelectedStatusIndex
        {
            get => _selectedStatusIndex;
            set
            {
                var clamped = Math.Clamp(value, 0, StatusFilters.Count - 1);
                if (!SetProperty(ref _selectedStatusIndex, clamped))
                    return;

                _selectedStatusFilter = StatusFilters[clamped];
                OnPropertyChanged(nameof(SelectedStatusFilter));

                if (_suppressStatusFilterReload || !_initialized)
                    return;

                _ = ApplyStatusFilterAsync();
            }
        }

        public StatusFilterOption SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                // Picker / bindings can push null when "All" is shown — never allow null.
                var next = value ?? StatusFilterOption.All;
                var index = IndexOfStatusFilter(next);
                // Drive through index setter so filter id + reload stay in sync
                SelectedStatusIndex = index;
            }
        }

        public async Task InitializeAsync()
        {
            EnsureCountdownTimer();
            if (_initialized && Launches.Count > 0)
                return;

            _initialized = true;
            // Avoid Picker rebinding "All" kicking off a second load that races the first
            _suppressStatusFilterReload = true;
            try
            {
                SelectedStatusIndex = 0;
                await LoadLaunchesAsync();
            }
            finally
            {
                _suppressStatusFilterReload = false;
            }
        }

        [RelayCommand]
        private async Task LoadLaunchesAsync()
        {
            await LoadPageAsync(reset: true, forceRefresh: true);
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            _activeSearch = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText.Trim();
            await LoadPageAsync(reset: true, forceRefresh: true);
        }

        [RelayCommand]
        private async Task ClearSearchAsync()
        {
            SearchText = string.Empty;
            _activeSearch = null;
            await LoadPageAsync(reset: true, forceRefresh: true);
        }

        private async Task ApplyStatusFilterAsync()
        {
            if (!_initialized)
                return;
            await LoadPageAsync(reset: true, forceRefresh: true);
        }

        [RelayCommand(CanExecute = nameof(CanLoadMore))]
        private async Task LoadMoreLaunchesAsync()
        {
            // Throttle load-more (point 6)
            var since = DateTime.UtcNow - _lastLoadMoreUtc;
            if (since < AppConfig.LoadMoreMinInterval)
                return;

            await LoadPageAsync(reset: false, forceRefresh: false);
        }

        private bool CanLoadMore() => HasMoreItems && !IsBusy;

        [RelayCommand]
        private async Task OpenLaunchAsync(LaunchListItemViewModel? item)
        {
            if (item is null || string.IsNullOrWhiteSpace(item.Id))
                return;

            await Shell.Current.GoToAsync(
                $"LaunchDetailPage?LaunchId={Uri.EscapeDataString(item.Id)}");
        }

        private async Task LoadPageAsync(bool reset, bool forceRefresh)
        {
            if (IsBusy)
                return;
            if (!reset && !HasMoreItems)
                return;

            CancelInFlight();
            _loadCts = new CancellationTokenSource();
            var ct = _loadCts.Token;

            IsBusy = true;
            ErrorMessage = null;
            ErrorDetail = null;
            ShowErrorDetail = false;
            IsFromCacheNotice = false;

            if (reset)
            {
                IsRefreshing = true;
                _offset = 0;
            }
            else
            {
                IsLoadingMore = true;
                _lastLoadMoreUtc = DateTime.UtcNow;
            }

            try
            {
                var statusId = _selectedStatusFilter?.Id;
                _diagnostics.Info(
                    _title,
                    $"Loading limit={AppConfig.PageSize} offset={(reset ? 0 : _offset)} search={_activeSearch ?? "-"} status={statusId?.ToString() ?? "all"} force={forceRefresh}");

                var page = await _pageFetcher(
                    _launchService,
                    AppConfig.PageSize,
                    reset ? 0 : _offset,
                    _activeSearch,
                    statusId,
                    null,
                    forceRefresh,
                    ct);

                ct.ThrowIfCancellationRequested();

                if (reset)
                    Launches.Clear();

                foreach (var launch in page.Items)
                    Launches.Add(new LaunchListItemViewModel(launch));

                _offset = page.NextOffset;
                TotalCount = page.TotalCount;
                HasMoreItems = page.HasNextPage && page.Items.Count > 0;
                TickCountdowns();
                UpdateStatusUi();
                _diagnostics.Info(_title, $"Loaded {page.Items.Count} items (total={page.TotalCount}, hasNext={page.HasNextPage})");
            }
            catch (OperationCanceledException)
            {
                // superseded by a newer request — do not surface as UI error
            }
            catch (Exception ex)
            {
                // Ignore cancel-like failures from resilience/Refit wrappers
                if (IsCancellation(ex))
                    return;

                ErrorMessage = FriendlyError(ex);
                ErrorDetail = FormatExceptionDetail(ex);
                _diagnostics.Error(_title, ErrorMessage, ex);
                if (reset && Launches.Count == 0)
                    HasMoreItems = false;
                UpdateStatusUi();
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
                IsLoadingMore = false;
                UpdateStatusUi();
            }
        }

        private void CancelInFlight()
        {
            try
            {
                _loadCts?.Cancel();
                _loadCts?.Dispose();
            }
            catch { /* ignore */ }
            _loadCts = null;
        }

        private void EnsureCountdownTimer()
        {
            if (_countdownTimer is not null)
                return;

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher is null)
                return;

            _countdownTimer = dispatcher.CreateTimer();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += (_, _) => TickCountdowns();
            _countdownTimer.Start();
        }

        private void TickCountdowns()
        {
            // Snapshot avoids "collection was modified" if a load updates Launches mid-tick
            var snapshot = Launches.ToArray();
            var now = DateTime.UtcNow;
            foreach (var item in snapshot)
                item.RefreshTimes(now);
        }

        private int IndexOfStatusFilter(StatusFilterOption option)
        {
            for (var i = 0; i < StatusFilters.Count; i++)
            {
                if (ReferenceEquals(StatusFilters[i], option)
                    || StatusFilters[i].Id == option.Id
                    && StatusFilters[i].Name == option.Name)
                {
                    return i;
                }
            }

            return 0; // All
        }

        private static bool IsCancellation(Exception ex)
            => ex is OperationCanceledException
               || ex is TaskCanceledException
               || (ex is AggregateException agg && agg.InnerExceptions.All(IsCancellation));

        private void UpdateStatusUi()
        {
            ShowEndOfList = !HasMoreItems && Launches.Count > 0;

            var searchBit = string.IsNullOrEmpty(_activeSearch)
                ? string.Empty
                : $" · \"{_activeSearch}\"";

            if (TotalCount <= 0)
            {
                StatusText = Launches.Count == 0
                    ? string.Empty
                    : $"Showing {Launches.Count}{searchBit}";
                return;
            }

            StatusText = $"Showing {Launches.Count} of {TotalCount}{searchBit}";
        }

        [RelayCommand]
        private void ToggleErrorDetail()
            => ShowErrorDetail = !ShowErrorDetail;

        [RelayCommand]
        private async Task CopyErrorAsync()
        {
            var text = string.IsNullOrWhiteSpace(ErrorDetail)
                ? ErrorMessage
                : $"{ErrorMessage}\n\n{ErrorDetail}";
            if (string.IsNullOrWhiteSpace(text))
                return;

            try
            {
                await Clipboard.Default.SetTextAsync(text);
                _diagnostics.Info(_title, "Error details copied to clipboard");
            }
            catch (Exception ex)
            {
                _diagnostics.Error(_title, "Failed to copy error", ex);
            }
        }

        [RelayCommand]
        private async Task OpenDiagnosticsAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("//Diagnostics");
            }
            catch (Exception ex)
            {
                _diagnostics.Error(_title, "Navigation to Diagnostics failed", ex);
            }
        }

        private static string FriendlyError(Exception ex)
        {
            // Refit reports HTTP 200 + ApiException when JSON deserialization fails
            if (ex is Refit.ApiException api)
            {
                if (api.InnerException is System.Text.Json.JsonException
                    || api.Content?.Contains("JsonException", StringComparison.OrdinalIgnoreCase) == true
                    || (int)api.StatusCode == 200 && api.InnerException is not null)
                {
                    return "Could not parse launch data from the API. Pull to refresh.";
                }

                return api.StatusCode switch
                {
                    HttpStatusCode.TooManyRequests =>
                        "Rate limited. Wait a moment, or set DUSTS_LL_API_TOKEN. Pull to refresh.",
                    HttpStatusCode.NotFound =>
                        "Launch data endpoint not found.",
                    _ => $"Could not load launches ({(int)api.StatusCode}). Pull to refresh."
                };
            }

            if (ex is System.Text.Json.JsonException)
                return "Could not parse launch data from the API. Pull to refresh.";

            if (ex is HttpRequestException)
                return "Network error — showing cache if available. Check connection.";

            if (ex is TaskCanceledException)
                return "Request timed out. Pull to refresh.";

            return string.IsNullOrWhiteSpace(ex.Message)
                ? "Something went wrong loading launches."
                : ex.Message;
        }

        private static string FormatExceptionDetail(Exception ex)
        {
            if (ex is Refit.ApiException api)
            {
                return
                    $"Type: {ex.GetType().FullName}\n" +
                    $"Status: {(int)api.StatusCode} {api.StatusCode}\n" +
                    $"Uri: {api.Uri}\n" +
                    $"Message: {api.Message}\n" +
                    $"Content: {Trim(api.Content, 1500)}\n" +
                    $"Inner: {api.InnerException}\n" +
                    $"Stack:\n{api.StackTrace}";
            }

            return ex.ToString();
        }

        private static string Trim(string? value, int max)
        {
            if (string.IsNullOrEmpty(value))
                return "(empty)";
            return value.Length <= max ? value : value[..max] + "…";
        }

        public void OnDisappearing()
        {
            // Keep timer running while on list; cancel network only
            CancelInFlight();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            CancelInFlight();
            if (_countdownTimer is not null)
            {
                _countdownTimer.Stop();
                _countdownTimer = null;
            }
        }
    }
}
