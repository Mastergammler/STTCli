using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class Setup : IDesignTimeDbContextFactory<SttContext>
{
    public SttContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.local.json").Build();
        var connectionString = configuration["ConnectionString"];

        var dbOptions = new DbContextOptionsBuilder<SttContext>();
        dbOptions.UseSqlServer(connectionString);
        var context = new SttContext(dbOptions.Options);

        return context;
    }
}