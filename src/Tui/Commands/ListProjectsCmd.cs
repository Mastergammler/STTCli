
using static Repl;
using static Symbols;

public class ListProjectsCmd(SttContext db, SttCache cache, TimeRepository times) : ICommand
{
    public void Execute(Memory<string> args)
    {
        var tags = db.Tags.ToArray();
        IQueryable<ListItem> query = db.Projects.OrderByDescending(i => i.Finished)
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

        var projects = query.ToArray();
        var projectIds = projects.Select(p => p.Id).ToHashSet();

        var taskCounts = db.Items.Where(i => i.Parent != null && projectIds.Contains(i.Parent.Id))
                                 .GroupBy(i => i.Parent.Id)
                                 .Select(g => new
                                 {
                                     ProjectId = g.Key,
                                     Finished = g.Count(i => i.Finished != null),
                                     Count = g.Count(),
                                     Percentage = Math.Round((float)g.Count(i => i.Finished != null) / g.Count() * 100, 2)
                                 })
                                 .ToDictionary(i => i.ProjectId);
        var projectTimes = times.AccumulationsFor(projectIds);

        // ------------ PRINTING ---------------------
        AsciiTable table = new();
        var now = Time.Now();

        //TODO: better way of unifying table definition
        table.AddColumns(("Name", false), ("Tags", false));
        table.AddColumn<TimeSpan>("Time", true, t => t.TotalSeconds > 0 ? t.Hours() : NO_DATA);
        table.AddColumn<(int, int, double)>("Tasks", true, t => $"{t.Item3}% {UNI_DASH} {t.Item1} / {t.Item2}");
        table.AddColumn<DateTime?>("Deadline", true, DEADLINE_FORMAT_NOTIME,
                                   new ColumnStyle<DateTime?>()
                                   {
                                       //Overdue
                                       StyleCondition = d => d <= now,
                                       BackgroundColorId = 52,
                                       IsRowStyle = true,
                                       Priority = 9
                                   },
                                   new ColumnStyle<DateTime?>()
                                   {
                                       // Within 7 days
                                       StyleCondition = d => d <= now.AddDays(7),
                                       BackgroundColorId = 58,
                                       IsRowStyle = true,
                                       Priority = 8
                                   },
                                   new ColumnStyle<DateTime?>()
                                   {
                                       // Within 30 days
                                       StyleCondition = d => d <= now.AddDays(30),
                                       TextColorId = 178,
                                       IsRowStyle = true,
                                       Priority = 7
                                   });
        table.AddColumn<DateTime?>("Finished", true, d => d?.NamedDate(),
                                    new ColumnStyle<DateTime?>()
                                    {
                                        StyleCondition = d => d is not null,
                                        BackgroundColorId = 22,
                                        IsRowStyle = true,
                                        Priority = 10
                                    });

        table.AddData(projects.Select(i => new object[]
        {
            i.Name.Truncate(36,true),
            DisplayTags(tags, i.Tags).Truncate(24,true),
            projectTimes.GetValueOrDefault(i.Id),
            taskCounts.ContainsKey(i.Id) ? (taskCounts[i.Id].Finished,taskCounts[i.Id].Count,taskCounts[i.Id].Percentage) : (0,0,0),
            i.Deadline,
            i.Finished,
        //TODO: sort by done items / percentage properly
        }).OrderByDescending(o => o[5]));
        table.Print(DEFAULT_INDENT);
    }
}