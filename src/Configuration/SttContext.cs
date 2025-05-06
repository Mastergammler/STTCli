using Microsoft.EntityFrameworkCore;

public class SttContext(DbContextOptions<SttContext> options) : DbContext(options)
{
    public DbSet<ListItem> Items { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Filter> Filters { get; set; }
}