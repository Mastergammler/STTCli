public class ReplCommands(ICommandFactory factory, Repl repl) : SubCommand
{
    protected override void InitCommands()
    {
        _commands["stats"] = factory.Create<DbStatsCmd>();
        _commands["show"] = factory.Create<ShowListCmd>();
        _commands["list"] = factory.Create<ListCmd>();

        _commands["new"] = factory.Create<CreateCmd>();
        _commands["edit"] = factory.Create<EditCmd>();
        _commands["delete"] = factory.Create<DeleteItemCmd>();
        _commands["finish"] = factory.Create<FinishCmd>();
        _commands["deadline"] = factory.Create<DeadlineCmd>();
        _commands["connect"] = factory.Create<ConnectCmd>();
        _commands["project"] = factory.Create<ProjectContextCmd>();

        // time tracking
        _commands["start"] = factory.Create<StartTrackingCmd>();
        _commands["stop"] = factory.Create<StopTrackingCmd>();
        _commands["tracking"] = factory.Create<TrackingInfoCmd>();

        _commands["exit"] = factory.Create<QuitCmd>();
    }
}