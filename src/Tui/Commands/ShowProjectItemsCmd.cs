
using static Repl;
using static Symbols;

public class ShowProjectItemsCmd(ProjectRepository projects, TagRepository tags, UiContext ctx) : ICommand
{
    public void Execute(Memory<string> args)
    {
        long? projectId = null;
        if (args.Length == 0 && ctx.CurrentProject is not null)
        {
            projectId = ctx.CurrentProject.Id;
        }
        else if (args.Length < 1 || args.Span.Contains("help"))
        {
            Print("Usage: project <projectName>");
            return;
        }

        string projectName;
        IEnumerable<ListItem> entities;

        if (projectId is not null)
        {
            projectName = ctx.CurrentProject.Name;
            entities = [projects.Get(projectId.Value)];
        }
        else
        {
            projectName = args.Span[0];
            entities = projects.FindByName(projectName);
        }

        if (ValidateSingleMatch(projectName, entities, e => e.Name))
        {
            var project = entities.Single();
            var tasks = projects.FindTasks(project);

            var finishedTasks = tasks.Where(t => t.IsFinished).Count();
            double percentage = Math.Round((float)finishedTasks / tasks.Count() * 100, 2);

            Print("");
            Print($"[{project.Name}] - {finishedTasks}/{tasks.Count()} ({percentage}%)");

            //TODO: refactor, not pretty, duplicate from list items cmd
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
                DisplayTags(tags.All(), i.Tags).Truncate(24,true),
                i.Finished
            }));
            table.Print(DEFAULT_INDENT);
        }
    }
}