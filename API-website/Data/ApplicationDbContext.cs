using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Video> Videos { get; set; }
    public DbSet<UserVideo> UserVideos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка таблицы UserVideo как связи многие ко многим
        modelBuilder.Entity<UserVideo>()
            .HasKey(uv => new { uv.UserId, uv.VideoId }); // Композитный ключ

        modelBuilder.Entity<UserVideo>()
            .HasOne(uv => uv.User)
            .WithMany(u => u.UserVideos)
            .HasForeignKey(uv => uv.UserId);

        modelBuilder.Entity<UserVideo>()
            .HasOne(uv => uv.Video)
            .WithMany(v => v.UserVideos)
            .HasForeignKey(uv => uv.VideoId);
    }
}
