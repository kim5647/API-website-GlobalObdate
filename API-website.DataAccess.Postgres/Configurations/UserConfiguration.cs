using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API_website.DataAccess.Postgres.Entities;

public class UserConfiguration : IEntityTypeConfiguration<UserEntities>
{
    public void Configure(EntityTypeBuilder<UserEntities> builder)
    {
        builder.HasKey(u => u.Id);

        builder
            .HasMany(u => u.Videos)
            .WithOne(v => v.User)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
  