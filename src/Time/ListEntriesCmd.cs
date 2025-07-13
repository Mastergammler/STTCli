
using static Repl;

public class ListEntriesCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        AsciiTable table = new();
        table.AddColumns("ID");
        table.AddColumn<DateTime>("Start", true, d => d.Time());
        table.AddColumn<DateTime?>("End", true, d => d?.Time());
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