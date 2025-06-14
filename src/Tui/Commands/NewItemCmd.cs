
using static Repl;

public class NewItemCmd(SttRepository repo, UiContext ctx, int level) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp("Usage: add <name> #<tag1> #<tag2> ...")) return;

        string name = args.Span[0];
        var tags = args.ToArray().Where(p => p.StartsWith("#")).ToArray();
        var msg = repo.CreateItem(name, tags, level, ctx.CurrentProject?.Id);

        Print(msg);
    }
}