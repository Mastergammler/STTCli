using Microsoft.EntityFrameworkCore;

public class SttContext(DbContextOptions<SttContext> options) : DbContext(options)
{
}