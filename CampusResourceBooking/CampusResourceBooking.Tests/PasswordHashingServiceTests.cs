using CampusResourceBooking.Services;

namespace CampusResourceBooking.Tests;

[TestClass]
public sealed class PasswordHashingServiceTests
{
    [TestMethod]
    public void Verify_ReturnsTrue_ForMatchingPassword()
    {
        var service = new PasswordHashingService();
        var hash = service.ComputeSha256Hash("student123");

        Assert.IsTrue(service.Verify("student123", hash));
    }

    [TestMethod]
    public void Verify_ReturnsFalse_ForDifferentPassword()
    {
        var service = new PasswordHashingService();
        var hash = service.ComputeSha256Hash("student123");

        Assert.IsFalse(service.Verify("wrong-password", hash));
    }
}
