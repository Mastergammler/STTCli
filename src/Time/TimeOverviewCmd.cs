using static Repl;
using static Symbols;

public class TimeOverviewCmd(TimeRepository repo, TagRepository tags) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp($"Usage: <timespan expression> [{GROUP_ARG}] [{SHORT_ARG}] [<filterExpr>]")) return;

        string timespanExpr = args.Span[0];
        bool groupItems = args.Span.Contains(GROUP_ARG);
        bool shortGroups = args.Span.Contains(SHORT_ARG);

        var timespan = Parsing.ParseTimespan(timespanExpr)
                              .Ensure(t => (t.End - t.Start).Days >= 1, t => $"Invalid timespan: < {t.Start.ShortDate()} - {t.End.ShortDate()} >");
        var filterOpts = Parsing.ParseFiltering(tags.All(), args);

        timespan.Execute(t =>
        {
            if (groupItems)
            {
                PrintGrouping(t, filterOpts, shortGroups);
            }
            else
            {
                PrintAccumulation(t, filterOpts);
            }
        });
    }

    private void PrintAccumulation(TimePeriod t, FilterOptions filterOpts)
    {
        TimeSpan ts = t.End - t.Start;
        //TODO: this doesn't take the parent project tag into account, should it?
        var entries = repo.FindItemsWithin(t.Start, t.End, filterOpts);

        if (ts.Days == 1)
        {
            Print($"< {t.Start.ShortDate()} >");
            Print("");
            foreach (var e in entries) Print(e.Entry.LocalFormat());

            Print("");
            Print("--- ACCUMULATED ---");
        }
        else
        {
            Print($"< {t.Start.ShortDate()} - {t.End.ShortDate()} >");
        }

        PrintAccumulated(entries);
    }

    private void PrintGrouping(TimePeriod t, FilterOptions filterOpts, bool shortDisplay)
    {
        // we don't want to filter the items here, 
        // we just want to group them based on the tags!
        var entries = repo.FindItemsWithin(t.Start, t.End, new());
        Print($"< {t.Start.ShortDate()} - {t.End.ShortDate()} >");

        List<TimeGroup> groups = [];
        HashSet<EntryData> usedEntries = [];

        //TODO: REF - ugly
        foreach (TagSet ts in filterOpts.Tags)
        {
            var tagEntries = entries.Where(e => Grouping.ByExpression(ts, e));
            usedEntries.AddAll(tagEntries);
            groups.Add(new(ts.Name, tagEntries.Select(e => e.Entry).Total(), Grouping.ByTask(tagEntries)));
        }

        // order for all the categories we want, but #rest should be the last
        var restGroup = entries.Except(usedEntries);
        groups = groups.OrderByDescending(g => g.Time).ToList();
        groups.Add(new("rest", restGroup.Select(e => e.Entry).Total(), Enumerable.Empty<TimeGroup>()));

        Grouping.PrintGroups(groups, shortDisplay ? 0 : 1);
    }

    private void PrintAccumulated(IEnumerable<EntryData> entries)
    {
        var total = entries.Select(e => e.Entry).Total();
        var projects = Grouping.ByProject(entries);
        var tasks = Grouping.ByTask(entries.Where(e => e.Meta.ProjectName is null));

        Grouping.PrintGroups(projects.Concat(tasks).OrderByDescending(g => g.Time), 1);
        Print($"--- Total {total.Hours()} ---");
    }
}