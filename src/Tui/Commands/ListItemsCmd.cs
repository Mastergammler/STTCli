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

        List<IQueryable<ListItem>> queries = [];

        foreach (TagSet ts in filter.Tags)
        {
            var subQuery = query.Where(i => (i.Tags & ts.Tags) == ts.Included);
            queries.Add(subQuery);
        }

        if (queries.Any())
        {
            //PERF: not sure if this is a performant query
            // but i guess its fine, because it's mostly only 1-3 items or something
            query = queries.Aggregate((a, b) => a.Concat(b));
        }

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