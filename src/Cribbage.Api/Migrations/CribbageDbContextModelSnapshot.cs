using Cribbage.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable
namespace Cribbage.Api.Migrations;

[DbContext(typeof(CribbageDbContext))]
partial class CribbageDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.11");
        modelBuilder.Entity("Cribbage.Api.Domain.Game", b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.Property<string>("Notes").HasMaxLength(1000).HasColumnType("character varying(1000)");
            b.Property<DateTimeOffset>("PlayedAt").HasColumnType("timestamp with time zone");
            b.Property<string>("PlayerOne").IsRequired().HasMaxLength(100).HasColumnType("character varying(100)");
            b.Property<int>("PlayerOneScore").HasColumnType("integer");
            b.Property<string>("PlayerTwo").IsRequired().HasMaxLength(100).HasColumnType("character varying(100)");
            b.Property<int>("PlayerTwoScore").HasColumnType("integer");
            b.HasKey("Id"); b.HasIndex("PlayedAt"); b.ToTable("games");
        });
    }
}
