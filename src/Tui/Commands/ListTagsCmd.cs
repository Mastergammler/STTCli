
using static Repl;

public class ListTagsCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        AsciiTable table = new();
        table.AddColumns("ID", "Name", "Bit");
        table.AddData(db.Tags.Select(i => new object[]
        {
            i.ID,
            i.Name,
            i.Bit // printing as Binary gets very long very fast
        }));
        table.Print(DEFAULT_INDENT);
    }
}