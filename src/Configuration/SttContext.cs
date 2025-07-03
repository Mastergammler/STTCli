using Microsoft.EntityFrameworkCore;

public class SttContext(DbContextOptions<SttContext> options) : DbContext(options)
{
    public DbSet<ListItem> Items { get; set; }
    public IQueryable<ListItem> Tasks => Set<ListItem>().Where(l => l.Level == 0);
    public IQueryable<ListItem> Projects => Set<ListItem>().Where(l => l.Level == 1);
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Filter> Filters { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    public IQueryable<TimeEntry> ActiveEntries => Set<TimeEntry>().Where(e => e.End == null);
}