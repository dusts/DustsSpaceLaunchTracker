namespace DustsSpaceLaunchTracker.ViewModels
{
    /// <summary>Common Launch Library status filters (point 11).</summary>
    public sealed class StatusFilterOption
    {
        public StatusFilterOption(string name, int? id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; }
        public int? Id { get; }

        public override string ToString() => Name;

        public static StatusFilterOption All { get; } = new("All statuses", null);

        public static IReadOnlyList<StatusFilterOption> AllOptions { get; } =
        [
            All,
            new("Go for Launch", 1),
            new("To Be Determined", 2),
            new("Success", 3),
            new("Failure", 4),
            new("On Hold", 5),
            new("In Flight", 6),
            new("Partial Failure", 7),
            new("To Be Confirmed", 8),
        ];
    }
}
