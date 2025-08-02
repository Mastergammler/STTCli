using static Repl;
using static Symbols;

record SetDeadlineOpt(IEnumerable<ListItem> items, DateTime deadline, bool force);

public class DeadlineCmd(ItemService service) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp($"Usage: deadline <$id|keyword> <dateExpr> [{OVERRIDE_ARG}] [{BULK_ARG}]", 2)) return;

        bool forceOverride = args.Span.Contains(OVERRIDE_ARG);
        bool allowBulk = args.Span.Contains(BULK_ARG);

        string searchStr = args.Span[0];
        string dateExpr = args.Span[1];

        var deadlineResult = Source.Of(dateExpr).MapNotNull(Parsing.ParseDateOld, e => INVALID_DATE_ERR.With(e));
        service.FindItems(new(searchStr, allowBulk))
               .Combine(deadlineResult, (i, d) => new SetDeadlineOpt(i, d, forceOverride))
               .Execute(SetDeadline);
    }

    // NOTE: D01 
    private void SetDeadline(SetDeadlineOpt inputs)
    {
        var group = inputs.items.ToLookup(i => i.Deadline != null && !inputs.force);
        var withoutDeadline = group[false];

        foreach (var item in withoutDeadline) item.Deadline = inputs.deadline;
        foreach (var item in group[true])
        {
            Print($"[!] Item {item.Id} has already a deadline. (Use {OVERRIDE_ARG} to override)");
        }

        service.SaveChanges();
        Print($"Updated {withoutDeadline.Count()}/{inputs.items.Count()} items {withoutDeadline.Ids()}");
    }
}