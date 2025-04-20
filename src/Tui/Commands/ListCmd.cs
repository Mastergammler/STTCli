using static Repl;

public class ListCmd(SttContext db, SttCache cache) : ICommand
{
    public const int DEFAULT_INDENT = 4;

    public void Execute(Memory<string> args)
    {
        if (args.Length == 0)
        {
            // TODO: generalized command handling etc
            // -> How would i define the commands?
            string[] options = ["items", "tags", "recent"];
            Print($"Select a type of list to show, options are:");
            Print($"{string.Join(',', options)}");
            return;
        }

        switch (args.Span[0])
        {
            case "items": ShowItemList(args.Slice(1)); break;
            case "tags": ShowTagList(args.Slice(1)); break;
            case "recent": ShowRecent(args.Slice(1)); break;
            default: Print($"Unknown list type '{args.Span[0]}'"); break;
        }
    }

    public void ShowRecent(Memory<string> input)
    {
        if (cache.RecentItems is not null && input.Length > 0)
        {
            cache.RecentItems.Filter("Name", input.Span[0]);
            cache.RecentItems.Print(DEFAULT_INDENT);
        }
        //TODO: This smells
        else Print("No recent table found or invalid filter!");
    }

    public void ShowItemList(Memory<string> input)
    {
        //PERF: can i cache these somewhere, instead doing this for every call?
        // Or does EF already handle caching for those quite well?
        var tags = db.Tags.ToArray();
        IQueryable<ListItem> query = db.Items.OrderByDescending(i => i.Finished);

        if (input.Span.Contains("-f"))
        {
            query = query.Where(i => i.Finished != null);
        }
        else if (!input.Span.Contains("-a"))
        {
            query = query.Where(i => i.Finished == null);
        }

        // if there are multiple filters, it needs to be always pairs
        var filterValue = input.Where(s => s.StartsWith("#"));
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

    public void ShowTagList(Memory<string> input)
    {
        AsciiTable table = new();
        table.AddColumns("ID", "Name", "Bit");
        table.AddData(db.Tags.Select(i => new object[]
        {
            i.ID,
            i.Name,
            i.Bit // printing as Binary gets very long very fast
        }));
        table.Print(DEFAULT_INDENT);
    }
}