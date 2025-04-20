using static Repl;

public class CreateItemCmd(SttRepository repo) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length < 1) Print("Usage: add <name> #<tag1> #<tag2> ...");

        string name = args.Span[0];
        var tags = args.ToArray().Where(p => p.StartsWith("#")).ToArray();
        var msg = repo.CreateItem(name, tags);

        Print(msg);
    }
}