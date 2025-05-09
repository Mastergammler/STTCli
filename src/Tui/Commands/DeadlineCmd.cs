using static Repl;

public class DeadlineCmd(SttContext db) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length < 2) { Print("Usage: deadline <searchStr> <date> [-force]"); return; }

        string searchStr = args.Span[0];
        string dateExpr = args.Span[1];
        bool forceOverride = args.Span.Contains("-force");

        //TODO: SEARCH - i should probably only search open items here?
        // -> How to integrate it generally?
        var foundItems = db.Items.Where(i => i.Finished == null)
                                 .Where(i => i.Name.ToLower().Contains(searchStr.ToLower()))
                                 .ToArray();
        if (foundItems.Length == 0)
        {
            Print($"No items matching search string '{searchStr}'");
            return;
        }
        else if (foundItems.Length > 1)
        {
            Print("Found more than 1 item, search needs to be more specific!");
            return;
        }

        var item = foundItems.Single();

        if (item.Deadline is not null && !forceOverride)
        {
            Print($"Item with id {item.Id} already has a deadline. Use -force to override!");
            return;
        }

        var deadline = Parsing.ParseDate(dateExpr);

        if (deadline is not null)
        {
            item.Deadline = deadline;
            db.SaveChanges();

            Print($"Added deadline {deadline} to item: {item.Name}");
        }
        else
        {
            Print($"Unable to interpret {dateExpr} as a valid date");
        }
    }
}