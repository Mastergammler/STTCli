public interface ICommandFactory
{
    ICommand Create<T>() where T : ICommand;
}

public class CommandFactory : ICommandFactory
{
    private SttContext _db;
    private SttRepository _repo;
    private SttCache _cache = new();

    public CommandFactory(SttContext db)
    {
        _db = db;
        _repo = new SttRepository(_db);
    }

    public ICommand Create<T>() where T : ICommand
    {
        return typeof(T) switch
        {
            Type t when t == typeof(ListCmd) => new ListCmd(this),
            Type t when t == typeof(ListItemsCmd) => new ListItemsCmd(_db, _cache),
            Type t when t == typeof(ListRecentCmd) => new ListRecentCmd(_cache),
            Type t when t == typeof(ListFiltersCmd) => new ListFiltersCmd(_db),
            Type t when t == typeof(ListTagsCmd) => new ListTagsCmd(_db),
            Type t when t == typeof(DbStatsCmd) => new DbStatsCmd(_db),
            Type t when t == typeof(CreateItemCmd) => new CreateItemCmd(_repo),
            Type t when t == typeof(DeleteItemCmd) => new DeleteItemCmd(_db),
            Type t when t == typeof(FinishCmd) => new FinishCmd(_db),
            Type t when t == typeof(ShowListCmd) => new ShowListCmd(_db, this),
            Type t when t == typeof(NewFilterCmd) => new NewFilterCmd(_db),
            _ => throw new InvalidOperationException($"Undefined command for type {typeof(T)}")
        };
    }
}