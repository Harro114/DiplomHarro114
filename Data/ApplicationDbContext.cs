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
    public DbSet<ExpUsers> ExpUsers { get; set; }

    // Метод настройки моделей
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связи Account - ExpUsers (один-к-одному)
        modelBuilder.Entity<Accounts>(z =>
            {
                z.HasKey(e => e.AccountId);
            }
        ); // Указание внешнего ключа


        // Настройка связи ExpChanges - Accounts (многие-к-одному)
        modelBuilder.Entity<ExpUsers>(z =>
        {
            z.HasKey(e => e.Id);
            z.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId);
        });


        // Настройка связи ExpChanges - ExpUsers (многие-к-одному)
        modelBuilder.Entity<ExpChanges>(z =>
        {
            z.HasKey(e => e.Id);
            z.HasOne(e => e.ExpUser)
                .WithMany()
                .HasForeignKey(e => e.ExpUserId);
            z.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId);
        });
        // Отношение без каскада
    }
}