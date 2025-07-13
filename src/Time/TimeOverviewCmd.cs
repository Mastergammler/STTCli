using static Repl;

public class TimeOverviewCmd(TimeRepository repo, TagRepository tags) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp("Usage: <timespan expression>")) return;

        string timespanExpr = args.Span[0];

        var timespan = Parsing.ParseTimespan(timespanExpr);
        var filterOpts = Parsing.ParseOptions(tags.All(), args);

        timespan.Execute(t =>
        {
            TimeSpan ts = t.end - t.start;
            if (ts.Days > 1)
            {
                var entries = repo.FindItemsWithin(t.start, t.end, filterOpts);
                Print($"< {t.start.ShortDate()} - {t.end.ShortDate()} >");
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
                Print($"Invalid timespan: < {t.start.ShortDate()} - {t.end.ShortDate()} >");
            }
        });
    }

    private void PrintForPeriod(DateTime from, DateTime to, FilterOptions opts)
    {
        var entries = repo.FindItemsWithin(from, to, opts);

        Print("");
        foreach (var entry in entries)
        {
            Print(entry.LocalFormat());
        }

        Print("");
        Print("--- ACCUMULATED ---");
        PrintAccumulated(entries);
    }

    private void PrintAccumulated(IEnumerable<TimeEntry> entries)
    {
        Print("");

        var total = entries.Total();
        var grouped = entries.GroupBy(e => e.Item.Name)
                             .Select(g => (g.Key, g.Total()))
                             .OrderByDescending(g => g.Item2);

        foreach (var group in grouped)
        {
            Print($"{group.Item2.Hours()}  {group.Item1}");
        }

        Print("");
        Print($"--- Total {total.Hours()} ---");
    }
}