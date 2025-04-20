using static Repl;

public class ListCmd : ICommand
{
    private Dictionary<string, ICommand> _commands;

    public ListCmd(ICommandFactory factory)
    {
        _commands = new()
        {
            ["items"] = factory.Create<ListItemsCmd>(),
            ["tags"] = factory.Create<ListTagsCmd>(),
            ["recent"] = factory.Create<ListRecentCmd>()
        };
    }

    public void Execute(Memory<string> args)
    {
        if (args.Length == 0)
        {
            Print($"Select a type of list to show, options are:");
            Print($"{string.Join(',', _commands.Keys)}");
            return;
        }

        if (_commands.ContainsKey(args.Span[0]))
        {
            _commands[args.Span[0]].Execute(args.Slice(1));
        }
        else
        {
            Print($"Unknown list type '{args.Span[0]}'");
        }
    }
}