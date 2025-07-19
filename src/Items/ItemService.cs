using static Symbols;

public record FindItemOptions(string searchStr, bool allowBulk = false, bool includeFinished = false);

public class ItemService(ItemRepository items, UiContext ctx)
{
    public Result<IEnumerable<ListItem>> FindItems(FindItemOptions opt)
    {
        Result<IEnumerable<ListItem>> result;

        string searchStr = opt.searchStr;
        if (ctx.WithinProjectContext && opt.searchStr.Equals(KEYWORD_THIS))
        {
            searchStr = $"${ctx.CurrentProject?.Id}";
        }

        if (searchStr.StartsWith(S_ID))
        {
            result = Source.Of(searchStr[1..])
                           .MapNotNull(ParseAsNullable, i => INVALID_NUM_ERR.With(i))
                           .Map(l => Collection.Of(items.Get(l)));
        }
        else
        {
            QueryFilter filter = new()
            {
                ProjectId = ctx.CurrentProject?.Id,
                IncludeFinished = opt.includeFinished
            };
            result = Source.Of(items.FindByName(searchStr, filter))
                           .Ensure(i => i.Any(), i => NO_ITEMS_MSG.With(opt.searchStr))
                           .Ensure(i => i.Count() == 1 || opt.allowBulk, i => NO_BULK_MSG.With(i.Names()));
        }

        return result;

    }


    private long? ParseAsNullable(string input) => long.TryParse(input, out var result) ? result : null;

    //TODO: Not Pretty -> passthrough only ...
    public void SaveChanges() => items.SaveChanges();
}