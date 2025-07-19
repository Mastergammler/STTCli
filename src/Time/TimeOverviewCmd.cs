using static Repl;

public class TimeOverviewCmd(TimeRepository repo, TagRepository tags) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp("Usage: <timespan expression>")) return;

        string timespanExpr = args.Span[0];

        var timespan = Parsing.ParseTimespan(timespanExpr)
                              .Ensure(t => (t.end - t.start).Days >= 1, t => $"Invalid timespan: < {t.start.ShortDate()} - {t.end.ShortDate()} >");
        var filterOpts = Parsing.ParseFiltering(tags.All(), args);

        timespan.Execute(t =>
        {
            TimeSpan ts = t.end - t.start;
            var entries = repo.FindItemsWithin(t.start, t.end, filterOpts);

            if (ts.Days == 1)
            {
                Print($"< {t.start.ShortDate()} >");
                Print("");
                foreach (var e in entries) Print(e.Entry.LocalFormat());

                Print("");
                Print("--- ACCUMULATED ---");
            }
            else
            {
                Print($"< {t.start.ShortDate()} - {t.end.ShortDate()} >");
            }

            PrintAccumulated(entries);
        });
    }

    private static Func<IEnumerable<EntryData>, IEnumerable<(string, TimeSpan)>> TaskGroup = list =>
        list.GroupBy(e => e.Meta.TaskName)
            .Select(g => (g.Key, g.Select(e => e.Entry).Total()));

    private void PrintAccumulated(IEnumerable<EntryData> entries)
    {
        Print("");

        var total = entries.Select(e => e.Entry).Total();
        var projects = entries.Where(e => e.Meta.ProjectName is not null)
                             .GroupBy(e => e.Meta.ProjectName)
                             .Select(g => (g.Key, g.Select(e => e.Entry).Total(), TaskGroup(g)));
        var tasks = TaskGroup(entries.Where(e => e.Meta.ProjectName is null))
            .Select(p => (p.Item1, p.Item2, Enumerable.Empty<(string, TimeSpan)>()));

        var ansiStyle = AnsiStyling.Text(Ansi256Color.GRAY);

        foreach (var group in projects.Concat(tasks).OrderByDescending(g => g.Item2))
        {
            Print($"{group.Item2.Hours()}  {group.Item1}", true);
            foreach (var sub in group.Item3)
            {
                //Print($"       {ansiStyle}{sub.Item1} ({sub.Item2.Hours().Trim()})", true);
                Print($"    {ansiStyle}{sub.Item2.Hours()}  {sub.Item1}", true);
            }
        }

        Print("", true);
        Print($"--- Total {total.Hours()} ---");
    }
}