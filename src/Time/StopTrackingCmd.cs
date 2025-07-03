using static Repl;

public class StopTrackingCmd(TimeService service) : ICommand
{
    public void Execute(Memory<string> args)
    {
        var stoppedEntries = service.StopTracking();
        if (stoppedEntries.Any())
        {
            Print($"Stopped tracking for: {stoppedEntries.Info()}");
        }
        else
        {
            Print("No entries currently active.");
        }
    }
}