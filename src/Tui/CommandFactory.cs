public interface ICommandFactory
{
    void Init(Repl repl);
    ICommand Create<T>() where T : ICommand;
}

public class CommandFactory : ICommandFactory
{
    private bool _initalized;

    private Repl _repl;
    private SttContext _db;
    private SttRepository _repo;
    private TagRepository _tags;
    private ProjectRepository _projects;
    private ItemRepository _items;
    private ItemService _itemService;
    private TimeService _timeService;
    private TimeRepository _timeRepository;
    private SttCache _cache = new();

    private UiContext _context = new();

    public CommandFactory(SttContext db)
    {
        _db = db;
        _repo = new SttRepository(_db);
        _tags = new TagRepository(_db);
        _projects = new ProjectRepository(_db);
        _items = new ItemRepository(_db);
        _itemService = new ItemService(_items, _context);
        _timeRepository = new TimeRepository(_db);
        _timeService = new TimeService(_timeRepository, _db);
    }

    public void Init(Repl repl)
    {
        _repl = repl;
        _initalized = true;
    }

    public ICommand Create<T>() where T : ICommand
    {
        if (!_initalized) throw new InvalidOperationException("CommandFactory has to be initalized before it can be used!");

        return typeof(T) switch
        {
            Type t when t == typeof(ReplCommands) => new ReplCommands(this, _repl),
            Type t when t == typeof(QuitCmd) => new QuitCmd(_repl, _timeService, _timeRepository),

            Type t when t == typeof(ListCmd) => new ListCmd(this),
            Type t when t == typeof(ListItemsCmd) => new ListItemsCmd(_items, _tags, 0),
            Type t when t == typeof(ListProjectsCmd) => new ListProjectsCmd(_items, _tags, _timeRepository),
            Type t when t == typeof(ListRecentCmd) => new ListRecentCmd(_cache),
            Type t when t == typeof(ListFiltersCmd) => new ListFiltersCmd(_db),
            Type t when t == typeof(ListTagsCmd) => new ListTagsCmd(_db),
            Type t when t == typeof(DbStatsCmd) => new DbStatsCmd(_db),

            Type t when t == typeof(ShowProjectItemsCmd) => new ShowProjectItemsCmd(_projects, _tags, _timeRepository, _context),
            Type t when t == typeof(ProjectContextCmd) => new ProjectContextCmd(_db, _context, _repl),
            Type t when t == typeof(ProjectCommands) => new ProjectCommands(this, _context, _repl),

            Type t when t == typeof(CreateCmd) => new CreateCmd(this),
            Type t when t == typeof(ConnectCmd) => new ConnectCmd(_db, _repo),
            Type t when t == typeof(NewItemCmd) => new NewItemCmd(_repo, _context, 0),
            Type t when t == typeof(NewProjectCmd) => new NewProjectCmd(_repo, _context),
            Type t when t == typeof(DeleteItemCmd) => new DeleteItemCmd(_db),
            Type t when t == typeof(FinishCmd) => new FinishCmd(_itemService),
            Type t when t == typeof(ShowListCmd) => new ShowListCmd(_db, this),
            Type t when t == typeof(NewFilterCmd) => new NewFilterCmd(_db),
            Type t when t == typeof(DeadlineCmd) => new DeadlineCmd(_itemService),
            Type t when t == typeof(EditCmd) => new EditCmd(this),
            Type t when t == typeof(EditItemsCmd) => new EditItemsCmd(_itemService, _tags),

            Type t when t == typeof(StartTrackingCmd) => new StartTrackingCmd(_itemService, _timeService),
            Type t when t == typeof(StopTrackingCmd) => new StopTrackingCmd(_timeService),
            Type t when t == typeof(TrackingInfoCmd) => new TrackingInfoCmd(_timeRepository),
            Type t when t == typeof(TimeOverviewCmd) => new TimeOverviewCmd(_timeRepository, _tags),
            Type t when t == typeof(TimeFillCmd) => new TimeFillCmd(_itemService, _timeService),
            Type t when t == typeof(ListEntriesCmd) => new ListEntriesCmd(_db),

            _ => throw new InvalidOperationException($"Undefined command for type {typeof(T)}")
        };
    }
}