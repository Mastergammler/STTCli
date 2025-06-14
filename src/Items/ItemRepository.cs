public class ItemRepository(SttContext db)
{
    public ListItem Get(long id) => db.Items.Single(i => i.Id == id);
    public ListItem? Find(long id) => db.Items.SingleOrDefault(i => i.Id == id);
    public void SaveChanges() => db.SaveChanges();

    public IEnumerable<ListItem> FindByName(string keyword, QueryFilter? filter = null)
    {
        var searchStr = keyword.ToLower();

        IQueryable<ListItem> items = db.Items;

        if (filter is not null)
        {
            if (filter.ProjectId is not null) items = items.Where(i => i.ParentId == filter.ProjectId);
            if (!filter.IncludeFinished) items = items.Where(i => i.Finished == null);
        }

        return items.Where(i => i.Name.ToLower().Contains(searchStr))
                    .ToArray();
    }
}