using System.Security.Cryptography;
using System.Text;

namespace CampusResourceBooking.Services;

public sealed class PasswordHashingService
{
    public string ComputeSha256Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public bool Verify(string password, string expectedHash) =>
        string.Equals(ComputeSha256Hash(password), expectedHash, StringComparison.OrdinalIgnoreCase);
}
