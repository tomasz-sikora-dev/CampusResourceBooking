using CampusResourceBooking.Services;

namespace CampusResourceBooking.Tests;

[TestClass]
public sealed class BookingTimeValidatorTests
{
    private readonly BookingTimeValidator validator = new();

    [TestMethod]
    public void Validate_ReturnsError_WhenEndIsBeforeStart()
    {
        var now = new DateTime(2026, 1, 1, 8, 0, 0);
        var startsAt = new DateTime(2026, 1, 2, 10, 0, 0);
        var endsAt = new DateTime(2026, 1, 2, 9, 0, 0);

        var errors = validator.Validate(startsAt, endsAt, now);

        Assert.IsTrue(errors.Any(error => error.Contains("zakończenia", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Validate_ReturnsError_WhenBookingIsTooLong()
    {
        var now = new DateTime(2026, 1, 1, 8, 0, 0);
        var startsAt = new DateTime(2026, 1, 2, 8, 0, 0);
        var endsAt = new DateTime(2026, 1, 2, 20, 0, 0);

        var errors = validator.Validate(startsAt, endsAt, now);

        Assert.IsTrue(errors.Any(error => error.Contains("8 godzin", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Validate_ReturnsNoErrors_ForValidRange()
    {
        var now = new DateTime(2026, 1, 1, 8, 0, 0);
        var startsAt = new DateTime(2026, 1, 2, 10, 0, 0);
        var endsAt = new DateTime(2026, 1, 2, 12, 0, 0);

        var errors = validator.Validate(startsAt, endsAt, now);

        Assert.AreEqual(0, errors.Count);
    }
}
