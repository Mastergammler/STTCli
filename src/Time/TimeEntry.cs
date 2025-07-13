public class TimeEntry
{
    public long Id { set; get; }
    public DateTime Start { set; get; }
    public DateTime? End { set; get; }

    public long ItemId { set; get; }
    public ListItem Item { set; get; }
}

public static class EntryExtensions
{
    public static TimeSpan Duration(this TimeEntry entry) => (entry.End ?? Time.Now()) - entry.Start;
    public static string LocalFormat(this TimeEntry entry)
    {
        return $"{entry.Start.Time()}  {entry.Item.Name} ({entry.Duration().Hours().Trim()})";
    }

    public static TimeSpan Total(this IEnumerable<TimeEntry> grouping)
        => grouping.Aggregate(TimeSpan.Zero, (acc, e) => acc + e.Duration());
}