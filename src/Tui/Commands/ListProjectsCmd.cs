
using static Repl;
using static Symbols;


public class ListProjectsCmd(ItemRepository items, TagRepository tags, TimeRepository times) : ICommand
{
    public void Execute(Memory<string> args)
    {
        var filter = Parsing.ParseFiltering(tags.All(), args);
        filter.Level = 1;

        var query = items.FilterBy(filter);

        var projects = query.ToArray();
        var projectIds = projects.Select(p => p.Id).ToHashSet();
        var taskCounts = items.ProjectTasksFor(projectIds)
                              .ToDictionary(i => i.ProjectId);
        var projectTimes = times.AccumulationsFor(projectIds);

        // ------------ PRINTING ---------------------
        AsciiTable table = new();
        var now = Time.Now();

        //TODO: better way of unifying table definition
        table.AddColumns(("Name", false), ("Tags", false));
        table.AddColumn<TimeSpan>("Time", true, t => t.TotalSeconds > 0 ? t.Hours() : NO_DATA);
        table.AddColumn<ProjectTaskInfo>("Tasks", true, t => $"{t.Percentage}% {UNI_DASH} {t.FinishedTasks} / {t.TotalTasks}");
        table.AddColumn<DateTime?>("Deadline", true, DEADLINE_FORMAT_NOTIME,
                                   new ColumnStyle<DateTime?>()
                                   {
                                       //Overdue
                                       StyleCondition = d => d <= now,
                                       BackgroundColor = Ansi256Color.DARK_RED,
                                       IsRowStyle = true,
                                       Priority = 9
                                   },
                                    new ColumnStyle<DateTime?>()
                                    {
                                        //Within the week
                                        StyleCondition = d => d <= now.AddDays(7),
                                        BackgroundColor = Ansi256Color.ORANGE,
                                        IsRowStyle = true,
                                        Priority = 8
                                    },
                                   new ColumnStyle<DateTime?>()
                                   {
                                       // Within the month
                                       StyleCondition = d => d <= now.AddDays(30),
                                       BackgroundColor = Ansi256Color.DARK_YELLOW,
                                       IsRowStyle = true,
                                       Priority = 7
                                   },
                                   new ColumnStyle<DateTime?>()
                                   {
                                       // Within the quarter (roughly)
                                       StyleCondition = d => d <= now.AddDays(90),
                                       TextColor = Ansi256Color.YELLOW,
                                       IsRowStyle = true,
                                       Priority = 6
                                   });
        table.AddColumn<DateTime?>("Finished", true, d => d?.NamedDate(),
                                    new ColumnStyle<DateTime?>()
                                    {
                                        StyleCondition = d => d is not null,
                                        BackgroundColor = Ansi256Color.DARK_GREEN,
                                        IsRowStyle = true,
                                        Priority = 10
                                    });

        table.AddData(projects.Select(i => new object[]
        {
            i.Name.Truncate(36,true),
            DisplayTags(tags.All(), i.Tags).Truncate(24,true),
            projectTimes.GetValueOrDefault(i.Id),
            taskCounts.GetValueOrDefault(i.Id, new()),
            i.Deadline,
            i.Finished,
        //TODO: sort by done items / percentage properly
        }).OrderByDescending(o => o[5]));
        table.Print(DEFAULT_INDENT);
    }
}