using static Symbols;

public static class ListItemExtensions
{
    public static string Names(this IEnumerable<ListItem> items)
    {
        return $"{UL}{string.Join(UL, items.Select(i => i.Name))}";
    }

    public static string Info(this IEnumerable<TimeEntry> entries)
    {
        return $"{UL}{string.Join(UL, entries.Select(i => $"{i.Item.Name} >> {i.Start.TillNow()}"))}\n";
    }

    public static string Ids(this IEnumerable<ListItem> items)
    {
        return $"{string.Join(", ", items.Select(i => i.Id))}";
    }
}