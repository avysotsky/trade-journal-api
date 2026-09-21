using Microsoft.EntityFrameworkCore;
using TradeJournal.Api.Models;

namespace TradeJournal.Api.Data;

public sealed class TradeJournalDbContext(DbContextOptions<TradeJournalDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<TradeSide>(name: "trade_side");
        modelBuilder.HasPostgresEnum<TradeStatus>(name: "trade_status");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("accounts");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100);
            entity.Property(x => x.BaseCurrency).HasMaxLength(10);
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Trade>(entity =>
        {
            entity.ToTable("trades", table =>
            {
                table.HasCheckConstraint("ck_trades_quantity_positive", "\"Quantity\" > 0");
                table.HasCheckConstraint("ck_trades_entry_price_positive", "\"EntryPrice\" > 0");
                table.HasCheckConstraint("ck_trades_fees_non_negative", "\"Fees\" >= 0");
            });
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Symbol).HasMaxLength(40);
            entity.Property(x => x.Quantity).HasPrecision(28, 10);
            entity.Property(x => x.EntryPrice).HasPrecision(28, 10);
            entity.Property(x => x.ExitPrice).HasPrecision(28, 10);
            entity.Property(x => x.Fees).HasPrecision(28, 10);
            entity.Property(x => x.RealizedPnl).HasPrecision(28, 10);
            entity.Property(x => x.Notes).HasMaxLength(2000);
            entity.HasOne(x => x.Account)
                .WithMany(x => x.Trades)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.AccountId, x.OpenedAt });
            entity.HasIndex(x => new { x.Symbol, x.Status });
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tags");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(50);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.HasMany(x => x.Trades)
                .WithMany(x => x.Tags)
                .UsingEntity("trade_tags");
        });
    }
}
