using static Repl;

public class ProjectCommands(ICommandFactory factory, UiContext ctx, Repl repl) : SubCommand
{
    protected override void InitCommands()
    {
        _commands["info"] = factory.Create<ShowProjectItemsCmd>();
        _commands["ls"] = factory.Create<ShowProjectItemsCmd>();
        _commands["start"] = factory.Create<StartTrackingCmd>();
        _commands["finish"] = factory.Create<FinishCmd>();
        _commands["deadline"] = factory.Create<DeadlineCmd>();
        _commands["new"] = factory.Create<NewItemCmd>();
        _commands["edit"] = factory.Create<EditItemsCmd>();
        _commands["exit"] = new QuitCmd(repl, ctx);

    }

    private class QuitCmd(Repl parent, UiContext state) : ICommand
    {
        public void Execute(Memory<string> args)
        {
            if (args.Span.Contains("help"))
            {
                Print("exits the current project context");
            }
            else
            {
                parent.ResetInputMode();
                state.CurrentProject = null;
            }
        }
    }
}