public class ProjectTaskInfo
{
    public static Func<ProjectTaskInfo, double> TaskPercentage = info => Math.Round((float)info.FinishedTasks
                                                                                    / (info.TotalTasks > 0 ? info.TotalTasks : 1)
                                                                                    * 100, 2);
    public long ProjectId { get; init; }
    public int FinishedTasks { get; init; }
    public int TotalTasks { get; init; }
    public double Percentage => TaskPercentage(this);
};

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

    //TODO: 
    // - Include ordering in options
    // - Include by project as well? (Is this usefull?)
    public IQueryable<ListItem> FilterBy(FilterOptions opt)
    {
        IQueryable<ListItem> query = db.Items.Where(i => i.Level == opt.Level)
                                             .OrderByDescending(i => i.Finished)
                                             .ThenByDescending(i => i.Deadline != null)
                                             .ThenBy(i => i.Deadline);

        if (opt.FinishedOnly)
        {
            query = query.Where(i => i.Finished != null);
        }
        else if (!opt.IncludeAll)
        {
            query = query.Where(i => i.Finished == null);
        }

        //TODO: REF - looks quite akward this way of doing it
        // i think there must be a better way for this
        List<IQueryable<ListItem>> queries = [];
        foreach (TagSet ts in opt.Tags)
        {
            var subQuery = query.Where(i => (i.Tags & ts.Tags) == ts.Included);
            queries.Add(subQuery);
        }

        if (queries.Any())
        {
            //PERF: not sure if this is a performant query
            // but i guess its fine, because it's mostly only 1-3 items or something
            query = queries.Aggregate((a, b) => a.Concat(b));
        }

        if (opt.TopList)
        {
            query = query.Take(opt.Limit);
        }

        return query;
    }

    public IEnumerable<ProjectTaskInfo> ProjectTasksFor(HashSet<long> projectIds)
    {
        return db.Items.Where(i => i.Parent != null && projectIds.Contains(i.Parent.Id))
                       .GroupBy(i => i.Parent.Id)
                       .Select(g => new ProjectTaskInfo
                       {
                           ProjectId = g.Key,
                           FinishedTasks = g.Count(i => i.Finished != null),
                           TotalTasks = g.Count()
                       });
    }
}