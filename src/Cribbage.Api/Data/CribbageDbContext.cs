using Cribbage.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cribbage.Api.Data;

public sealed class CribbageDbContext(DbContextOptions<CribbageDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var game = modelBuilder.Entity<Game>();
        game.ToTable("games");
        game.HasKey(x => x.Id);
        game.Property(x => x.PlayerOne).HasMaxLength(100).IsRequired();
        game.Property(x => x.PlayerTwo).HasMaxLength(100).IsRequired();
        game.Property(x => x.Notes).HasMaxLength(1000);
        game.HasIndex(x => x.PlayedAt);
    }
}
