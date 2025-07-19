
using static Repl;

//TODO: #5 - refactor
public class ShowListCmd(SttContext db, ICommandFactory factory) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length < 1)
        {
            Print("Usage: show <searchString>");
            return;
        }

        string searchStr = args.Span[0];
        var entities = db.Filters.Where(f => f.Name.Contains(searchStr));

        if (!entities.Any())
        {
            Print($"No filter found matching {searchStr}");
        }
        else if (entities.Count() > 1)
        {
            Print($"Multiple matches found:\n{string.Join("\n - ", entities.Select(e => e.Name))}");
        }
        else
        {
            var filterEntity = entities.First();
            var cmd = factory.Create<ListItemsCmd>();
            var expression = Parsing.ParseCmdInput(filterEntity.Expression);
            cmd.Execute(expression);
        }
    }
}