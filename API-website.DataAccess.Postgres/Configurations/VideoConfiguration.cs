using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API_website.DataAccess.Postgres.Entities;

public class VideoConfiguration : IEntityTypeConfiguration<VideoEntities>
{
    public void Configure(EntityTypeBuilder<VideoEntities> builder)
    {
        builder.HasKey(v => v.Id);
    }
}