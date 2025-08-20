using CommandsService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommandsService;

public class CommandConfiguration: IEntityTypeConfiguration<Command>
{
  public void Configure(EntityTypeBuilder<Command> builder)
  {
    builder.ToTable("Commands");

    builder.Property(x => x.Id).ValueGeneratedOnAdd();

    builder.Property(x => x.HowTo).HasColumnType("VARCHAR(70)").HasMaxLength(70);

    builder.Property(x => x.CommandLine).HasColumnType("VARCHAR(120)").HasMaxLength(120);

    builder.HasOne(x => x.Platform)
    .WithMany(x => x.Commands)
    .HasForeignKey(x => x.PlatformId)
    .OnDelete(DeleteBehavior.Cascade)
    .IsRequired();
  }
}
