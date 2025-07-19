public abstract class SubCommand : ICommand
{
    protected Dictionary<string, ICommand> _commands = new();

    public SubCommand() { InitCommands(); }
    protected abstract void InitCommands();

    public void Execute(Memory<string> args)
    {
        if (_commands.Count() == 0) throw new InvalidOperationException("No sub commands specified! At least 1 command is required!");
        if (args.ShowHelp($"Following commands are available:\n   {string.Join(", ", _commands.Keys)}")) return;

        var subCmdStr = args.Span[0];

        Source.Of(_commands.Keys.Where(k => k.StartsWith(subCmdStr)))
              .Ensure(i => i.Any(), i => $"No command matches the input '{subCmdStr}'")
              .Ensure(i => i.Count() == 1, i => $"Ambiguous input, matches:{i.Ul()}")
              .Single()
              .Execute(cmd => _commands[cmd].Execute(args.Slice(1)));
    }
}