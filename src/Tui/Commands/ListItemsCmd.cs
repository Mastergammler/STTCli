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

        var filter = Parsing.ParseOptions(tags, args);
        // OR tag filter for AND tags
        if (filter.AndTags.Any()) query = query.Where(i => filter.AndTags.Any(t => (t & i.Tags) > 0));

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