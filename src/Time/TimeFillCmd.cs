
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
             .Combine(ParseTime(timeExpr), (i, d) => (i, d))
             .Combine(DetermineFillType(args), (tup, type) => new FillOptions(tup.i, tup.d, type))
             .Execute(opt =>
             {
                 time.FillItem(opt.item, opt.time, opt.type)
                     .Execute(e =>
                     {
                         Print($"Created entry {e.Start} - {e.End} for item: {e.Item.Name}");
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
              .Ensure(v => v.Single(), $"{START_ARG},{END_ARG} and {SURROUND_ARG} are mutually exclusive!")
              //TODO: bit ugly that i throw away this value now again and also don't switch on it
              .Map(v =>
                      isStartTime ? FillType.START :
                      isEndTime ? FillType.END :
                      isInBetween ? FillType.MIDDLE :
                      FillType.NONE
              );
    }

    private Result<DateTime> ParseTime(string timeExpr)
    {
        if (timeExpr.Length == 3 || timeExpr.Length == 4)
        {
            var splitIdx = timeExpr.Length == 4 ? 2 : 1;
            var hourStr = timeExpr[..splitIdx];
            var minStr = timeExpr[splitIdx..];
            var today = DateTime.UtcNow;

            var minResult = minStr.ParseInt(i => i >= 0 && i < 60, NUM_INVALID_MIN);
            return hourStr.ParseInt(i => i >= 0 && i < 24, NUM_INVALID_HOUR)
                          .Combine(minResult, (h, m) => new DateTime(today.Year, today.Month, today.Day, h, m, 0, DateTimeKind.Local))
                          .Map(d => d.ToUniversalTime());
        }
        else
        {
            return Result.Fail<DateTime>($"Time expression of length {timeExpr.Length} not supported.");
        }
    }
}