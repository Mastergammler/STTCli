using static Repl;

public class DbStatsCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        int itemCount = db.Items.Count();
        int tagCount = db.Tags.Count();

        Print($"The db holds:");
        Print($"- {itemCount} items");
        Print($"- {tagCount} tags");
    }
}