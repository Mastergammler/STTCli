
using static Repl;

public class EditItemsCmd(SttContext db, SttRepository repo) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.Length < 1 || args.Span.Contains("help"))
        {
            Print("Usage: <item name> [-f] [-b] ['>> <New Name>'] [#<newTag>] [-#<removeTag>]");
            return;
        }

        string searchStr = args.Span[0];

        var tags = db.Tags.ToArray();
        IQueryable<ListItem> query = db.Items.Where(i => i.Name.ToLower().Contains(searchStr.ToLower()));

        if (!args.Span.Contains("-f")) query = query.Where(i => i.Finished == null);

        var items = query.ToArray();

        if (items.Length == 0)
        {
            Print($"No items found with name '{searchStr}'");
            return;
        }
        else if (items.Length > 1 && !args.Span.Contains("-b"))
        {
            Print($"Found multiple items, but bulk option ('-b') not specified:\n - {string.Join("\n - ", items.Select(i => i.Name))}");
            return;
        }

        ICollection<string> addTags = [];
        ICollection<string> removeTags = [];
        string? newName = null;

        foreach (string s in args.Span)
        {
            if (s.StartsWith(">>"))
            {
                var name = s[2..].Trim();
                if (newName != null)
                {
                    Print($"Multiple names given, chosing '{name}'");
                }
                newName = name;
            }
            else if (s.StartsWith("#"))
            {
                addTags.Add(s);
            }
            else if (s.StartsWith("-#"))
            {
                removeTags.Add(s[1..]);
            }
        }

        var removeBitset = removeTags.Select(n => tags.FirstOrDefault(t => t.Name.Equals(n)))
                                     .Where(t => t is not null)
                                     .Aggregate(0L, (a, t) => a | t.Bit);
        var groups = addTags.Select(n => new { Tag = tags.FirstOrDefault(t => t.Name.Equals(n)), Name = n })
                            .ToLookup(i => i.Tag is not null);

        var addBitset = groups[true].Aggregate(0L, (a, i) => a | i.Tag.Bit);
        addBitset |= groups[false].Select(n => repo.CreateNextTag(n.Name)).Aggregate(0L, (a, t) => a | t.Bit);

        foreach (var item in items)
        {
            if (newName is not null) item.Name = newName;
            item.Tags = item.Tags & ~removeBitset;
            item.Tags = item.Tags | addBitset;
        }

        db.SaveChanges();
        Print($"Updated {items.Length} item(s)");
    }
}