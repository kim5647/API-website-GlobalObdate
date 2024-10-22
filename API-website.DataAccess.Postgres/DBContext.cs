using Microsoft.EntityFrameworkCore;


public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Video> Videos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new VideoConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
