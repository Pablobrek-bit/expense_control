using ExpenseControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseControl.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; } = null!;

    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Age).IsRequired();
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Description).IsRequired().HasMaxLength(500);
            entity.Property(t => t.Value).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(t => t.Type).IsRequired();

            entity.HasOne(t => t.Person)
                  .WithMany(p => p.Transactions)
                  .HasForeignKey(t => t.PersonId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
