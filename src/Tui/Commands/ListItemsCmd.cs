using static Repl;

public class ListItemsCmd(SttContext db, SttCache cache) : ICommand
{
    public void Execute(Memory<string> args)
    {
        //PERF: can i cache these somewhere, instead doing this for every call?
        // Or does EF already handle caching for those quite well?
        var tags = db.Tags.ToArray();
        IQueryable<ListItem> query = db.Items.OrderByDescending(i => i.Finished);

        if (args.Span.Contains("-f"))
        {
            query = query.Where(i => i.Finished != null);
        }
        else if (!args.Span.Contains("-a"))
        {
            query = query.Where(i => i.Finished == null);
        }

        // if there are multiple filters, it needs to be always pairs
        var filterValue = args.Where(s => s.StartsWith("#"));
        long filterTags = filterValue.Select(tagName => tags.SingleOrDefault(t => t.Name.Equals(tagName)))
                                     .Aggregate(0L, (sum, tag) => sum | tag?.Bit ?? 0);
        // OR tag filter
        if (filterTags > 0) query = query.Where(i => (i.Tags & filterTags) > 0);

        AsciiTable table = new();
        table.AddColumns(("ID", true), ("Name", false), ("Tags", false), ("Created", true), ("Finished", true));
        table.AddData(query.Select(i => new object[]
        {
            i.Id,
            i.Name,
            DisplayTags(tags, i.Tags),
            i.Created.ToString(SHORT_DATE),
            i.Finished != null ? i.Finished.Value.ToString(SHORT_DATE) : null
        }));
        table.Print(DEFAULT_INDENT);

        cache.RecentItems = table;
    }
}
