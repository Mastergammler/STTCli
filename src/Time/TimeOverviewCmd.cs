using static Repl;

public class TimeOverviewCmd(TimeRepository repo, TagRepository tags) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp("Usage: <timespan expression>")) return;

        string timespanExpr = args.Span[0];

        var timespan = Parsing.ParseTimespan(timespanExpr);
        var filterOpts = Parsing.ParseOptions(tags.All(), args);
        //TODO: UI -> date format
        timespan.Execute(t =>
        {
            TimeSpan ts = t.end - t.start;
            if (ts.Days > 1)
            {
                var entries = repo.FindItemsWithin(t.start, t.end, filterOpts);
                Print($"< {t.start.ToShortDateString()} - {t.end.ToShortDateString()} >");
                PrintAccumulated(entries);
            }
            else if (ts.Days == 1)
            {
                Print($"< {t.start.ToShortDateString()} >");
                PrintForPeriod(t.start, t.end, filterOpts);
            }
            else
            {
                //TODO: should probably happen before timespan is returned
                Print($"Invalid timespan: < {t.start.ToShortDateString()} - {t.end.ToShortDateString()} >");
            }
        });
    }

    private void PrintForPeriod(DateTime from, DateTime to, FilterOptions opts)
    {
        var entries = repo.FindItemsWithin(from, to, opts);

        Print("");
        foreach (var entry in entries)
        {
            Print(entry.ToString());
        }

        Print("");
        Print("--- ACCUMULATED ---");
        PrintAccumulated(entries);
    }

    private void PrintAccumulated(IEnumerable<TimeEntry> entries)
    {
        Print("");

        var total = entries.Aggregate(TimeSpan.Zero, (acc, e) => acc + e.Duration);
        var grouped = entries.GroupBy(e => e.Item.Name)
                             .Select(g => (g.Key, g.Aggregate(TimeSpan.Zero, (acc, e) => acc + e.Duration)))
                             .OrderByDescending(g => g.Item2);

        foreach (var group in grouped)
        {
            Print($"{group.Item2.Format()}  {group.Item1}");
        }

        Print("");
        Print($"--- Total {total.Format()} ---");
    }
}