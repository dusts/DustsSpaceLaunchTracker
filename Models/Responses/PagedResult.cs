namespace DustsSpaceLaunchTracker.Models.Responses
{
    /// <summary>
    /// App-level page of results (maps from API PaginatedResponse).
    /// </summary>
    public sealed class PagedResult<T>
    {
        public required IReadOnlyList<T> Items { get; init; }
        public int TotalCount { get; init; }
        public int Offset { get; init; }
        public int Limit { get; init; }
        public bool HasNextPage { get; init; }

        public int NextOffset => Offset + Items.Count;
    }
}
