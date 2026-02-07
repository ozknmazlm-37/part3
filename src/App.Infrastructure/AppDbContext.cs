using App.Core;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Cofre> Cofres => Set<Cofre>();
    public DbSet<Meter> Meters => Set<Meter>();
    public DbSet<CofrePassword> CofrePasswords => Set<CofrePassword>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(x => x.Username).IsUnique();
        modelBuilder.Entity<User>().HasIndex(x => x.TelegramUserId);
        modelBuilder.Entity<User>().HasIndex(x => x.TelegramChatId);
        modelBuilder.Entity<User>().HasIndex(x => x.WhatsAppId);

        modelBuilder.Entity<Cofre>().HasKey(x => x.CofreNo);
        modelBuilder.Entity<Cofre>().HasIndex(x => x.BuildingName);

        modelBuilder.Entity<Meter>().HasKey(x => x.MeterSerialNo);
        modelBuilder.Entity<Meter>().HasIndex(x => x.CofreNo);

        modelBuilder.Entity<CofrePassword>().HasIndex(x => x.CofreNo);
        modelBuilder.Entity<CofrePassword>().HasIndex(x => x.Status);

        modelBuilder.Entity<AuditLog>().HasIndex(x => x.Timestamp);
        modelBuilder.Entity<AuditLog>().HasIndex(x => x.EntityType);

        modelBuilder.Entity<AppSetting>().HasKey(x => x.Key);
    }
}
