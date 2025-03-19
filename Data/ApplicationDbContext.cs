using Diplom.Models;
using Microsoft.EntityFrameworkCore;

namespace Diplom.Data;

public class ApplicationDbContext : DbContext // Имя класса теперь согласовано
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options);
    
    
    public DbSet<Accounts> Accounts { get; set; }
    public DbSet<ExpChanges> ExpChanges { get; set; }
    public DbSet<ExpUsers> ExpUsers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accounts>().ToTable("Accounts");
        modelBuilder.Entity<ExpChanges>().ToTable("ExpChanges");
        modelBuilder.Entity<ExpUsers>().ToTable("ExpUsers");
        modelBuilder.Entity<ExpUsers>()
            .HasOne(e => e.Account)
            .HasForeignKey<Accounts>(a => a.AccountId);

        modelBuilder.Entity<ExpChanges>()
            .HasOne(e => e.Account)
            .WithOne()
            .HasForeignKey<Accounts>(a => a.AccountId);

        modelBuilder.Entity<ExpChanges>()
            .HasOne(e => e.ExpUser)
            .WithOne()
            .HasForeignKey<ExpUsers>(e => e.ExpUser);
    }
}