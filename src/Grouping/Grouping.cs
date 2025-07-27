using static Repl;

public record TimeGroup(string Name, TimeSpan Time, IEnumerable<TimeGroup> Children);

public static class Grouping
{
    public const Ansi256Color SUBGROUP_TEXT_COLOR = Ansi256Color.GRAY;
    public const string GROUP_INDENT = "    ";
    public const string ITEM_GAP = "  ";

    public static Func<IEnumerable<EntryData>, IEnumerable<TimeGroup>> ByTask = list =>
            list.GroupBy(e => e.Meta.TaskName)
                .Select(g => new TimeGroup(g.Key, g.Select(e => e.Entry).Total(), Enumerable.Empty<TimeGroup>()))
                .OrderByDescending(g => g.Time);

    public static Func<IEnumerable<EntryData>, IEnumerable<TimeGroup>> ByProject = list =>
           list.Where(e => e.Meta.ProjectName is not null)
               .GroupBy(e => e.Meta.ProjectName)
               .Select(g => new TimeGroup(g.Key, g.Select(e => e.Entry).Total(), Grouping.ByTask(g)));


    public static void PrintGroups(IEnumerable<TimeGroup> groups, int levels = 0)
    {
        Print("");

        var ansiStyle = AnsiStyling.Text(SUBGROUP_TEXT_COLOR);
        foreach (var group in groups)
        {
            Print($"{group.Time.Hours()}{ITEM_GAP}{group.Name}", true);
            if (levels > 0)
            {
                foreach (var sub in group.Children)
                {
                    Print($"{GROUP_INDENT}{ansiStyle}{sub.Time.Hours()}{ITEM_GAP}{sub.Name}", true);
                }
            }
        }

        Print("", true);
    }

    //TODO: Strategy Pattern?
    public static Func<TagSet, EntryData, bool> ByExpression = (ts, e) =>
    {
        long projectItemTags = e.Entry.Item.Tags | (e.Meta.ProjectTags ?? 0);

        if (ts.ExUnionExpr) return ts.ExclusionaryUnion(projectItemTags);
        else if (ts.AndExpr) return ts.AndMatch(projectItemTags);
        else return ts.OrMatch(projectItemTags);
    };
}