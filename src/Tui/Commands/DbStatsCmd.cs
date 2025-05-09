using static Repl;

public class DbStatsCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        int itemCount = db.Items.Count();
        int tagCount = db.Tags.Count();
        int filterCount = db.Filters.Count();

        Print($"The db holds:");
        Print($"- {itemCount} items");
        Print($"- {tagCount} tags");
        Print($"- {filterCount} filters");
        Print("Console info:");
        Print($"- {Console.LargestWindowWidth} Max");
        Print($"- {Console.WindowWidth} Current");
        Print($"- {Console.BufferWidth} Buffer");
    }
}