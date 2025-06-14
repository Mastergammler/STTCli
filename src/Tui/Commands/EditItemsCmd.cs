
using static Repl;
using static Symbols;

public class EditItemsCmd(ItemService service, TagRepository tags) : ICommand
{
    public void Execute(Memory<string> args)
    {
        if (args.ShowHelp($"Usage: <{S_ID}id|keyword> [{BULK_ARG}] [{FINISHED_ARG}] " +
                          $"['{S_NAME} <New Name>'] [{S_TAG}<newTag>] [{S_REM_TAG}<removeTag>]")) return;

        bool allowBulk = args.Span.Contains(BULK_ARG);
        bool includeFinished = args.Span.Contains(FINISHED_ARG);

        string searchStr = args.Span[0];

        service.FindItems(new(searchStr, allowBulk, includeFinished))
               .Execute(i => UpdateItemsData(i, args.Span));
    }

    private void UpdateItemsData(IEnumerable<ListItem> found, Span<string> args)
    {
        ICollection<string> addTags = [];
        ICollection<string> removeTags = [];
        string? newName = null;

        foreach (string s in args)
        {
            if (s.StartsWith(S_NAME))
            {
                var name = s[2..].Trim();
                if (newName != null)
                {
                    //TODO: REF - Should not be allowed ond throw an error instead
                    Print($"[!] Multiple names given, chosing '{name}'");
                }
                newName = name;
            }
            else if (s.StartsWith(S_TAG)) addTags.Add(s);
            else if (s.StartsWith(S_REM_TAG)) removeTags.Add(s[1..]);
        }

        var tagUpdate = new TagUpdate(tags);
        tagUpdate.Prepare(addTags, removeTags);

        foreach (var item in found)
        {
            if (newName is not null) item.Name = newName;
            tagUpdate.ApplyTo(item);
        }

        service.SaveChanges();
        Print($"Updated {found.Count()} item(s) ({found.Ids()})");
    }
}