
using HisaabApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HisaabApi.Data;

public class HisaabDbContext : DbContext
{
    public HisaabDbContext(
        DbContextOptions<HisaabDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(c => c.Type)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(c => c.Icon)
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Amount)
                .HasPrecision(18, 2);

            entity.Property(t => t.Type)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(t => t.Note)
                .HasMaxLength(500);

            entity.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
