using static Repl;

public class ListCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length == 0)
        {
            // TODO: generalized command handling etc
            // -> How would i define the commands?
            string[] options = ["items", "tags"];
            Print($"Select a type of list to show, options are:");
            Print($"{string.Join(',', options)}");
            return;
        }

        switch (args.Span[0])
        {
            case "items": ShowItemList(args.Slice(1)); break;
            case "tags": ShowTagList(args.Slice(1)); break;
            default: Print($"Unknown list type '{args.Span[0]}'"); break;
        }
    }

    public void ShowItemList(Memory<string> input)
    {
        //TODO: parse tag filters & filter items before querying

        var tags = db.Tags.ToArray();

        AsciiTable table = new();
        table.AddColumns("ID", "Name", "Tags", "Created", "Finished");
        table.AddData(db.Items.Select(i => new object[]
        {
            i.Id,
            i.Name,
            DisplayTags(tags, i.Tags),
            i.Created.ToString(DATE_FORMAT),
            i.Finished
        }));
        table.Print(4);
    }

    public void ShowTagList(Memory<string> input)
    {
        AsciiTable table = new();
        table.AddColumns("ID", "Name", "Bit");
        table.AddData(db.Tags.Select(i => new object[]
        {
            i.ID,
            i.Name,
            i.Bit // printing as Binary gets very long very fast
        }));
        table.Print(4);
    }


}