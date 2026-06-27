using Microsoft.EntityFrameworkCore;

namespace CampusResourceBooking.Models;

public sealed class CampusResourceBookingContext(DbContextOptions<CampusResourceBookingContext> options) : DbContext(options)
{
    public DbSet<CampusUser> CampusUsers => Set<CampusUser>();
    public DbSet<ResourceCategory> ResourceCategories => Set<ResourceCategory>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<VBookingFull> VBookingFull => Set<VBookingFull>();
    public DbSet<VBookingStatistics> VBookingStatistics => Set<VBookingStatistics>();
    public DbSet<VResourceUsage> VResourceUsage => Set<VResourceUsage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CampusUser>(entity =>
        {
            entity.ToTable("CampusUsers");
            entity.HasKey(user => user.CampusUserId);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(40);
        });

        modelBuilder.Entity<ResourceCategory>(entity =>
        {
            entity.ToTable("ResourceCategories");
            entity.HasKey(category => category.ResourceCategoryId);
            entity.HasIndex(category => category.Name).IsUnique();
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.ToTable("Resources");
            entity.HasKey(resource => resource.ResourceId);
            entity.HasOne(resource => resource.Category)
                .WithMany(category => category.Resources)
                .HasForeignKey(resource => resource.ResourceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Bookings");
            entity.HasKey(booking => booking.BookingId);
            entity.Property(booking => booking.Status).HasConversion<string>().HasMaxLength(40);
            entity.HasOne(booking => booking.Resource)
                .WithMany(resource => resource.Bookings)
                .HasForeignKey(booking => booking.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(booking => booking.RequestedBy)
                .WithMany(user => user.Bookings)
                .HasForeignKey(booking => booking.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(booking => booking.DecidedBy)
                .WithMany()
                .HasForeignKey(booking => booking.DecidedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VBookingFull>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vBookingFull");
        });

        modelBuilder.Entity<VBookingStatistics>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vBookingStatistics");
        });

        modelBuilder.Entity<VResourceUsage>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vResourceUsage");
        });
    }
}
