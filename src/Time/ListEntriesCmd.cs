
using static Repl;
using static Symbols;

public class ListEntriesCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        AsciiTable table = new();
        table.AddColumns("ID");
        table.AddColumn<DateTime>("Start", true, d => d.ToLocalTime().ToString(TIME_PORTION_ONLY));
        table.AddColumn<DateTime?>("End", true, d => d?.ToLocalTime().ToString(TIME_PORTION_ONLY));
        table.AddColumn<string>("Item name", false, s => s);
        table.AddData(db.TimeEntries.OrderByDescending(e => e.Start).Take(20).Select(i => new object[]
        {
            i.Id,
            i.Start,
            i.End,
            i.Item.Name
        }));
        table.Print(DEFAULT_INDENT);
    }
}