using Diplom.Models;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Data;

public class ApplicationDbContext : DbContext
{
    // Конструктор
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSet для каждой из моделей
    public DbSet<Accounts> Accounts { get; set; }
    public DbSet<ExpChanges> ExpChanges { get; set; }
    public DbSet<ExpUsersWallets> ExpUsersWallets { get; set; }
    public DbSet<Config> Config { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrdersHistory> OrdersHistory { get; set; }

    // Метод настройки моделей
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связи Account - ExpUsers (один-к-одному)
        modelBuilder.Entity<Accounts>(z => { z.HasKey(e => e.Id); }
        );


        // Настройка связи ExpChanges - Accounts (многие-к-одному)
        modelBuilder.Entity<ExpUsersWallets>(z =>
        {
            z.HasKey(e => e.Id);
            z.HasOne(eu => eu.Accounts)
                .WithOne(a => a.ExpUserWallets)
                .HasForeignKey<ExpUsersWallets>(eu => eu.AccountId);
        });


        // Настройка связи ExpChanges - ExpUsers (многие-к-одному)
        modelBuilder.Entity<ExpChanges>(z =>
        {
            z.HasKey(e => e.Id);
            z.HasOne<Accounts>(ec => ec.Accounts)
                .WithMany()
                .HasForeignKey(ec => ec.AccountId);
            z.HasOne<ExpUsersWallets>(ec => ec.ExpUsersWallets)
                .WithMany()
                .HasForeignKey(ec => ec.ExpUserId);
        });

        modelBuilder.Entity<Config>(z =>
        {
            z.HasNoKey();
        });

        modelBuilder.Entity<Orders>(z =>
        {
            z.HasKey(e => e.Id);
            z.HasOne<Accounts>(ec => ec.Accounts)
                .WithMany()
                .HasForeignKey(ec => ec.AccountId);
        });
        
        modelBuilder.Entity<OrdersHistory>(z =>
        {
            z.HasKey(e => e.Id);
            z.HasOne<Accounts>(ec => ec.Accounts)
                .WithMany()
                .HasForeignKey(ec => ec.AccountId);
            
        });
    }
}