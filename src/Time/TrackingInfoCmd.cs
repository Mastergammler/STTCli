using static Repl;
using static Symbols;

public class TrackingInfoCmd(TimeRepository repo) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp($"[{SILENT_ARG}]", 0)) return;

        var entries = repo.FindActiveItems();
        if (entries.Any())
        {
            Print($"{entries.Info()}");
        }
        else if (!args.Span.Contains(SILENT_ARG))
        {
            Print("No time tracking active currently.");
        }
    }
}