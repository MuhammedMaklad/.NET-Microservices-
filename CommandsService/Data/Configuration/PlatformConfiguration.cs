using CommandsService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommandsService.Data.Configuration;

public class PlatformConfiguration : IEntityTypeConfiguration<Platform>
{
  public void Configure(EntityTypeBuilder<Platform> builder)
  {
    builder.ToTable("Platforms");

    builder.Property(x => x.Id).ValueGeneratedOnAdd();

    builder.Property(x => x.ExternalId);

    builder.Property(x => x.Name).HasColumnType("VARCHAR(70)").HasMaxLength(70);

  }
}
