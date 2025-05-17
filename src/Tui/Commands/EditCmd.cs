public class EditCmd(ICommandFactory factory) : SubCommand
{
    protected override void InitCommands()
    {
        _commands["item"] = factory.Create<EditItemsCmd>();
    }
}