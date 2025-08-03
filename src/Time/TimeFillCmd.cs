
using static Repl;
using static Symbols;

record FillOptions(ListItem item, DateTime time, FillType type);

public class TimeFillCmd(ItemService items, TimeService time) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp($"Usage: <timeExpr> <keyword> [{START_ARG}|{END_ARG}|{SURROUND_ARG}]", 2)) return;

        string searchWord = args.Span[1];
        string timeExpr = args.Span[0];

        items.FindItems(new(searchWord)).Single()
             .Combine(Parsing.ParseTime(timeExpr), (i, d) => (i, d))
             .Combine(DetermineFillType(args), (tup, type) => new FillOptions(tup.i, tup.d, type))
             .Execute(opt =>
             {
                 time.FillItem(opt.item, opt.time, opt.type)
                     .Execute(e =>
                     {
                         Print($"Created entry {e.Start} - {e.End} for item: {e.Item.Name.WithBg(ITEM_BG)}");
                     });
             });
    }

    private Result<FillType> DetermineFillType(Memory<string> args)
    {
        bool isStartTime = args.Span.Contains(START_ARG);
        bool isEndTime = args.Span.Contains(END_ARG);
        bool isInBetween = args.Span.Contains(SURROUND_ARG);

        bool[] values = [isStartTime, isEndTime, isInBetween];

        return Source.Of(values)
              .Ensure(v => v.SingleTruth(), $"{START_ARG},{END_ARG} and {SURROUND_ARG} are mutually exclusive!")
              .Map(v =>
                      isStartTime ? FillType.START :
                      isEndTime ? FillType.END :
                      isInBetween ? FillType.MIDDLE :
                      FillType.DEFAULT
              );
    }
}