using static Repl;

public abstract class SubCommand : ICommand
{
    protected Dictionary<string, ICommand> _commands = new();

    public SubCommand() { InitCommands(); }
    protected abstract void InitCommands();

    public void Execute(Memory<string> args)
    {
        if (_commands.Count() == 0) throw new InvalidOperationException("No sub commands specified! At least 1 command is required!");

        if (args.Length == 0)
        {
            Print($"To few arguments: Need to select one of:");
            Print($"{string.Join(',', _commands.Keys)}");
            return;
        }

        if (_commands.ContainsKey(args.Span[0]))
        {
            _commands[args.Span[0]].Execute(args.Slice(1));
        }
        else
        {
            Print($"Unknown sub command: '{args.Span[0]}'");
        }
    }
}