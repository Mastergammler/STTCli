using static Symbols;

public static class ListItemExtensions
{

    public static string TimeDiff(this DateTime time)
    {
        var span = DateTime.UtcNow - time;
        return $"{(int)span.TotalHours:D2}:{(int)span.Minutes:D2} h";
    }

    public static string Names(this IEnumerable<ListItem> items)
    {
        return $"{UL}{string.Join(UL, items.Select(i => i.Name))}";
    }

    public static string Info(this IEnumerable<TimeEntry> entries)
    {
        return $"{UL}{string.Join(UL, entries.Select(i => $"{i.Item.Name} >> {i.Start.TimeDiff()}"))}\n";
    }

    public static string Ids(this IEnumerable<ListItem> items)
    {
        return $"{string.Join(", ", items.Select(i => i.Id))}";
    }
}