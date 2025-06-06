public class CreateCmd(ICommandFactory factory) : SubCommand
{
    protected override void InitCommands()
    {
        _commands["item"] = factory.Create<NewItemCmd>();
        _commands["project"] = factory.Create<NewProjectCmd>();
        _commands["filter"] = factory.Create<NewFilterCmd>();
    }
}