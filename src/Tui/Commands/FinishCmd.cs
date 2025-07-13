using static Repl;
using static Symbols;

record DateExprOpt(DateTime date, bool force);
record FinishItemOpt(IEnumerable<ListItem> items, DateExprOpt dateExpression);

public class FinishCmd(ItemService service) : ICommand
{
    public const string OVR_DATE = "--";
    public const string DATE_EXPR_ARG = $"{OVR_DATE}<date expr>";

    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp($"Usage: <$id|keyword> [{BULK_ARG}] [{DATE_EXPR_ARG}]")) return;

        bool allowBulk = args.Span.Contains(BULK_ARG);
        string searchParam = args.Span[0];

        var dateResult = args.FindArg(s => s.StartsWith(OVR_DATE))
                             .Ensure(c => c.Count() < 2, c => MULTI_ARG_ERR.With(DATE_EXPR_ARG))
                             .OneOrDefault(p => Source.Of(p[OVR_DATE.Length..])
                                                      .MapNotNull(Parsing.ParseDate, e => INVALID_DATE_ERR.With(e))
                                                      .Map(d => new DateExprOpt(d, true)),
                                           new DateExprOpt(Time.Now(), false));

        service.FindItems(new(searchParam, allowBulk, true))
               .Combine(dateResult, (i, d) => new FinishItemOpt(i, d))
               .Execute(FinishItems);
    }

    // NOTE: D01 
    private void FinishItems(FinishItemOpt inputs)
    {
        var group = inputs.items.ToLookup(i => i.Finished != null && !inputs.dateExpression.force);
        var unfinished = group[false];

        foreach (var item in unfinished) item.Finished = inputs.dateExpression.date;
        foreach (var item in group[true])
        {
            Print($"[!] Item {item.Id} is already finished! (Skipped)");
        }

        service.SaveChanges();
        Print($"Updated {unfinished.Count()}/{inputs.items.Count()} items {unfinished.Ids()}");
    }
}