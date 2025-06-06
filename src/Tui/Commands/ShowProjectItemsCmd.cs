
using static Repl;

public class ShowProjectItemsCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length < 1 || args.Span.Contains("help"))
        {
            Print("Usage: project <projectName>");
            return;
        }

        string projectName = args.Span[0].ToLower();

        var entities = db.Projects.Where(p => p.Name.ToLower().Contains(projectName));
        if (!entities.Any())
        {
            Print($"No matches fournd for '{projectName}'");
        }
        else if (entities.Count() > 1)
        {
            Print($"Multiple matches found:\n{string.Join("\n - ", entities.Select(e => e.Name))}");
        }
        else
        {
            var project = entities.Single();
            var tasks = db.Tasks.Where(p => p.Parent == project).ToArray();
            var finishedTasks = tasks.Where(t => t.IsFinished).Count();

            Print("");
            Print($"[{project.Name}] - {finishedTasks}/{tasks.Count()}");

            //TODO: refactor, not pretty, duplicate from list items cmd
            var tags = db.Tags.ToArray();
            AsciiTable table = new();

            table.AddColumns(("Name", false),
                             ("Tags", false));
            table.AddColumn<DateTime?>("Finished", true, d => d?.ToLocalTime().ToString(SHORT_DATE),
                                        new ColumnStyle<DateTime?>()
                                        {
                                            StyleCondition = d => d is not null,
                                            BackgroundColorId = 22,
                                            IsRowStyle = true,
                                            Priority = 10
                                        });

            table.AddData(tasks.Select(i => new object[]
            {
                i.Name.Truncate(36,true),
                DisplayTags(tags, i.Tags).Truncate(24,true),
                i.Finished
            }));
            table.Print(DEFAULT_INDENT);
        }
    }
}