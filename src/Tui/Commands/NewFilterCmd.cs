using static Repl;

public class NewFilterCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length < 2)
        {
            Print("Usage: new <listName> <expression>");
            return;
        }

        string name = args.Span[0];
        string expression = string.Join(' ', args.Slice(1).Span.ToArray());

        if (db.Filters.Count(f => f.Name.Equals(name)) > 0)
        {
            Print($"Filter with name {name} already exists.");
        }
        else
        {
            db.Add(new Filter { Name = name, Expression = expression });
            db.SaveChanges();
            Print($"Created new filter: {name}");
        }
    }
}