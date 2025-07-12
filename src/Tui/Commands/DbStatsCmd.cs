using static Repl;

public class DbStatsCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        int itemCount = db.Items.Count();
        int projectCount = db.Projects.Count();
        int taskCount = db.Tasks.Count();
        int tagCount = db.Tags.Count();
        int filterCount = db.Filters.Count();
        int timeEntries = db.TimeEntries.Count();

        Print($"The db holds:");
        Print($"- {itemCount} items ({taskCount} tasks, {projectCount} Projects)");
        Print($"- {tagCount} tags");
        Print($"- {filterCount} filters");
        Print($"- {timeEntries} time entries");
        Print("Console info:");
        Print($"- {Console.LargestWindowWidth} Max");
        Print($"- {Console.WindowWidth} Current");
        Print($"- {Console.BufferWidth} Buffer");
    }
}