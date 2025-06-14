public static class ListItemExtensions
{
    public const string UNI_BULLET = "\u2022";

    public static string Names(this IEnumerable<ListItem> items)
    {
        return $"\n {UNI_BULLET} {string.Join($"\n {UNI_BULLET} ", items.Select(i => i.Name))}";
    }

    public static string Ids(this IEnumerable<ListItem> items)
    {
        return $"{string.Join(", ", items.Select(i => i.Id))}";
    }
}