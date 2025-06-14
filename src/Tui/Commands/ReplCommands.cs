using static Repl;

public class ReplCommands(ICommandFactory factory, Repl repl) : SubCommand
{
    protected override void InitCommands()
    {
        _commands["stats"] = factory.Create<DbStatsCmd>();
        _commands["show"] = factory.Create<ShowListCmd>();
        _commands["list"] = factory.Create<ListCmd>();

        _commands["add"] = factory.Create<CreateCmd>();
        _commands["edit"] = factory.Create<EditCmd>();
        _commands["delete"] = factory.Create<DeleteItemCmd>();
        _commands["finish"] = factory.Create<FinishCmd>();
        _commands["deadline"] = factory.Create<DeadlineCmd>();
        _commands["connect"] = factory.Create<ConnectCmd>();
        _commands["project"] = factory.Create<ProjectContextCmd>();

        _commands["exit"] = new QuitCmd(repl);
    }

    private class QuitCmd(Repl parent) : ICommand
    {
        public void Execute(Memory<string> args)
        {
            if (args.ShowHelp("quits the cli", 0)) return;
            parent.Exit();
        }
    }
}