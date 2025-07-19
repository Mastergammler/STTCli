using static Repl;
using static Symbols;

public class ListItemsCmd(ItemRepository items, TagRepository tags, int level) : ICommand
{
    public void Execute(Memory<string> args)
    {
        var filter = Parsing.ParseFiltering(tags.All(), args);
        var query = items.FilterBy(filter);

        var now = Time.Now();
        AsciiTable table = new();

        //TODO: TUI - add setting max size value as well
        table.AddColumns(("ID", true),
                         ("Name", false),
                         ("Tags", false));

        table.AddColumn<DateTime>("Created", true, d => d.NamedDate());
        table.AddColumn<DateTime?>("Deadline", true, DEADLINE_FORMAT,
                                   new ColumnStyle<DateTime?>()
                                   {
                                       //Overdue
                                       StyleCondition = d => d <= now,
                                       BackgroundColor = Ansi256Color.DARK_RED,
                                       IsRowStyle = true,
                                       Priority = 9
                                   },
                                   new ColumnStyle<DateTime?>()
                                   {
                                       // Within 7 days
                                       StyleCondition = d => d <= now.AddDays(7),
                                       BackgroundColor = Ansi256Color.DARK_YELLOW,
                                       IsRowStyle = true,
                                       Priority = 8
                                   },
                                   new ColumnStyle<DateTime?>()
                                   {
                                       // Within 30 days
                                       StyleCondition = d => d <= now.AddDays(30),
                                       TextColor = Ansi256Color.YELLOW,
                                       IsRowStyle = true,
                                       Priority = 7
                                   });
        table.AddColumn<DateTime?>("Finished", true, d => d?.NamedDate(),
                                    new ColumnStyle<DateTime?>()
                                    {
                                        StyleCondition = d => d is not null,
                                        BackgroundColor = Ansi256Color.DARK_GREEN,
                                        IsRowStyle = true,
                                        Priority = 10
                                    });

        //TODO: need to make this configurable also
        // -> this should be in column definition, not the table definition ...
        table.AddData(query.Select(i => new object[]
        {
            i.Id,//.ToString().Truncate(4,true),
            //TODO: TUI - make it configurable & depending on max size
            i.Name.Truncate(36,true),
            DisplayTags(tags.All(), i.Tags).Truncate(24,true),
            //TODO: TUI - handle time conversion better
            i.Created.NamedDate(),
            i.Deadline,
            i.Finished
        }));
        table.Print(DEFAULT_INDENT);
    }
}