public class ProjectRepository(SttContext db)
{
    public ListItem Get(long projectId) => db.Projects.Single(p => p.Id == projectId);

    //TODO: add filter + sort options etc
    public IEnumerable<ListItem> FindByName(string keyword)
    {
        string searchNormalized = keyword.ToLower();
        return db.Projects.Where(p => p.Name.ToLower().Contains(searchNormalized));
    }

    public IEnumerable<ListItem> FindTasks(ListItem project) => db.Tasks.Where(t => t.Parent == project).ToArray();
}
