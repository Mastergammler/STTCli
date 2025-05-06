
using static Repl;

public class ListFiltersCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        AsciiTable table = new();
        table.AddColumns("ID", "Name", "Expression");
        table.AddData(db.Filters.Select(f => new object[]
        {
            f.Id,
            f.Name,
            f.Expression
        }));
        table.Print(DEFAULT_INDENT);
    }

}