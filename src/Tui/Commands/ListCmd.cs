public class ListCmd(ICommandFactory factory) : SubCommand
{
    protected override void InitCommands()
    {
        _commands["items"] = factory.Create<ListItemsCmd>();
        _commands["projects"] = factory.Create<ListProjectsCmd>();
        _commands["tags"] = factory.Create<ListTagsCmd>();
        _commands["recent"] = factory.Create<ListRecentCmd>();
        _commands["filters"] = factory.Create<ListFiltersCmd>();
    }
}