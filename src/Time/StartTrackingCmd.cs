
using static Repl;
using static Symbols;

public class StartTrackingCmd(ItemService items, TimeService service) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp($"Usage: <{S_ID}id|keyword>")) return;

        bool allowBulk = false;
        // it makes no sense to track on finished items?
        // -> Maybe for edit after the fact?
        bool includeFinished = false;

        string searchStr = args.Span[0];

        //TODO: this still would give the 'bulk' error msg, which is incorrect here
        items.FindItems(new(searchStr, allowBulk, includeFinished))
             .Single()
             .Ensure(i => i.Level == 0, i => TT_ONLY_FOR_TASKS_ERR.With(i.Level))
             .Execute(AddTimeEntries);
    }

    private void AddTimeEntries(ListItem item)
    {
        service.StartTracking(item);
        Print($"Started time tracking for item {item.Name.WithBg(ITEM_BG)}");
    }
}