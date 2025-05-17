using static Repl;

public class ListItemsCmd(SttContext db, SttCache cache) : ICommand
{
    public void Execute(Memory<string> args)
    {
        //PERF: can i cache these somewhere, instead doing this for every call?
        // Or does EF already handle caching for those quite well?
        var tags = db.Tags.ToArray();
        IQueryable<ListItem> query = db.Items.OrderByDescending(i => i.Finished)
                                             .ThenByDescending(i => i.Deadline != null)
                                             .ThenBy(i => i.Deadline);

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

        //TODO: TUI - add setting max size value as well
        table.AddColumns(("ID", true),
                         ("Name", false),
                         ("Tags", false),
                         ("Created", true));
        table.AddColumn<DateTime?>("Deadline", true, d => d?.ToLocalTime().ToString(SHORT_DATE),
                                   new ColumnStyle<DateTime?>()
                                   {
                                       //Overdue
                                       StyleCondition = d => d <= DateTime.UtcNow,
                                       BackgroundColorId = 52,
                                       IsRowStyle = true,
                                       Priority = 9
                                   },
                                   new ColumnStyle<DateTime?>()
                                   {
                                       // Within 7 days
                                       StyleCondition = d => d <= DateTime.UtcNow.AddDays(7),
                                       BackgroundColorId = 58,
                                       IsRowStyle = true,
                                       Priority = 8
                                   },
                                   new ColumnStyle<DateTime?>()
                                   {
                                       // Within 30 days
                                       StyleCondition = d => d <= DateTime.UtcNow.AddDays(30),
                                       TextColorId = 178,
                                       IsRowStyle = true,
                                       Priority = 7
                                   });
        table.AddColumn<DateTime?>("Finished", true, d => d?.ToLocalTime().ToString(SHORT_DATE),
                                    new ColumnStyle<DateTime?>()
                                    {
                                        StyleCondition = d => d is not null,
                                        BackgroundColorId = 22,
                                        IsRowStyle = true,
                                        Priority = 10
                                    });

        //TODO: need to make this configurable also
        // -> this should be in column definition, not the table definition ...
        table.AddData(query.Select(i => new object[]
        {
            i.Id,//.ToString().Truncate(4,true),
            //TODO: TUI - make it configurable & depending on max size
            i.Name.Truncate(36,true),
            DisplayTags(tags, i.Tags).Truncate(24,true),
            //TODO: TUI - handle time conversion better
            i.Created.ToLocalTime().ToString(SHORT_DATE),
            i.Deadline,
            i.Finished
        }));
        table.Print(DEFAULT_INDENT);

        cache.RecentItems = table;
    }
}