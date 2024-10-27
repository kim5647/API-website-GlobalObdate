using Microsoft.EntityFrameworkCore;
using API_website.DataAccess.Postgres.Entities;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {
    }

    public DbSet<UserEntities> Users { get; set; }
    public DbSet<VideoEntities> Videos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new VideoConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
