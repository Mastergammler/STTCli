

using static Repl;

public class ConnectCmd(SttContext db, SttRepository repo) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length < 2 || args.Span.Contains("help"))
        {
            Print("Usage: <item name> <projectName> [-b]");
            return;
        }

        string itemName = args.Span[0].ToLower();
        string projectName = args.Span[1].ToLower();

        var items = db.Tasks.Where(i => i.Name.ToLower().Contains(itemName));
        var projects = db.Projects.Where(i => i.Name.ToLower().Contains(projectName));

        if (projects.Count() == 0)
        {
            Print($"No project found for the name {projectName}");
            return;
        }
        else if (projects.Count() > 1)
        {
            Print($"Found more than one match for search '{projectName}':\n{string.Join("\n - ", projects.Select(i => i.Name))}");
            return;
        }
        else if (items.Count() == 0)
        {
            Print($"No items found for the name {itemName}");
            return;
        }
        else if (items.Count() > 1)
        {
            Print($"Found more than one match for search '{itemName}':\n{string.Join("\n - ", items.Select(i => i.Name))}");
            return;
        }

        ListItem project = projects.Single();
        ListItem item = items.Single();

        string result = repo.Connect(project, item);
        Print(result);
    }
}
