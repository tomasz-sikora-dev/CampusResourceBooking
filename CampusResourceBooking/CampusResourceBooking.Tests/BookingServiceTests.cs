using CampusResourceBooking.Dtos;
using CampusResourceBooking.Models;
using CampusResourceBooking.Services;
using Microsoft.EntityFrameworkCore;

namespace CampusResourceBooking.Tests;

[TestClass]
public sealed class BookingServiceTests
{
    [TestMethod]
    public async Task CreateBookingAsync_ReturnsFailure_WhenApprovedBookingOverlaps()
    {
        await using var db = CreateContext();
        await SeedAsync(db);
        var service = new BookingService(db, new BookingTimeValidator());

        var model = new BookingFormModel
        {
            ResourceId = 1,
            Purpose = "Spotkanie koła naukowego",
            Date = DateTime.Today.AddDays(5),
            StartsAt = new TimeSpan(10, 30, 0),
            EndsAt = new TimeSpan(11, 30, 0)
        };

        var result = await service.CreateBookingAsync(1, model);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Message.Contains("zarezerwowany", StringComparison.OrdinalIgnoreCase));
    }

    private static CampusResourceBookingContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CampusResourceBookingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CampusResourceBookingContext(options);
    }

    private static async Task SeedAsync(CampusResourceBookingContext db)
    {
        db.CampusUsers.Add(new CampusUser
        {
            CampusUserId = 1,
            FirstName = "Jan",
            LastName = "Student",
            Email = "student@campus.local",
            PasswordHash = "hash",
            Role = UserRole.Student,
            IsActive = true
        });

        db.ResourceCategories.Add(new ResourceCategory
        {
            ResourceCategoryId = 1,
            Name = "Sala"
        });

        db.Resources.Add(new Resource
        {
            ResourceId = 1,
            Name = "Sala 1",
            Building = "A",
            RoomNumber = "101",
            Capacity = 20,
            IsActive = true,
            RequiresApproval = true,
            ResourceCategoryId = 1
        });

        db.Bookings.Add(new Booking
        {
            BookingId = 1,
            ResourceId = 1,
            RequestedByUserId = 1,
            StartsAt = DateTime.Today.AddDays(5).AddHours(10),
            EndsAt = DateTime.Today.AddDays(5).AddHours(11),
            Purpose = "Istniejąca rezerwacja",
            Status = BookingStatus.Approved
        });

        await db.SaveChangesAsync();
    }
}
