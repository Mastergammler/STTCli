using static Symbols;

public static class ListItemExtensions
{
    public static string TimeDiff(this DateTime time)
    {
        var span = DateTime.UtcNow - time;
        return span.Format();
    }

    public static string Format(this TimeSpan span)
    {
        if (span.Hours > 0)
        {
            string hours = ((int)span.TotalHours).ToString().PadLeft(2, ' ');
            return $"{hours}:{span.Minutes:D2} h";
        }
        else
        {
            return $"   {span.Minutes.ToString().PadLeft(2, '0')} m";
        }
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