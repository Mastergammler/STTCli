
using static Repl;

public class ListRecentCmd(SttCache cache) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length == 0)
        {
            Print("Usage is recent <filter>");
            return;
        }

        if (cache.RecentItems is not null)
        {
            cache.RecentItems.Filter("Name", args.Span[0]);
            cache.RecentItems.Print(DEFAULT_INDENT);
        }
        else Print("No recent table found!");
    }
}