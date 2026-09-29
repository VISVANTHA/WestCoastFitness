using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Api.Models;

namespace WestCoastFitness.Api.Data;

public class FitnessClubDbContext(DbContextOptions<FitnessClubDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>()
            .HasIndex(m => m.Email)
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.MemberId, b.ClassSessionId })
            .IsUnique();

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Member)
            .WithMany(m => m.Bookings)
            .HasForeignKey(b => b.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.ClassSession)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.ClassSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
