using static Repl;
using static Symbols;

public class QuitCmd(Repl parent, TimeService service, TimeRepository repo) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp("quits the cli [-y|-n]", 0)) return;

        bool no = args.Span.Contains(NO_ARG);
        bool yes = args.Span.Contains(YES_ARG);

        var entries = repo.FindActiveItems();
        if (entries.Any() && !no)
        {
            if (yes || StoppingConfirmed())
            {
                service.StopTracking();
            }
        }

        parent.Exit();
    }

    private bool StoppingConfirmed()
    {
        Print($"There are active time entries: Confirm to stop (y)");
        var input = Console.ReadKey(true);
        if (input.Key == ConsoleKey.Y)
        {
            return true;
        }
        return false;
    }
}