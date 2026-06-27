using CampusResourceBooking.Dtos;
using CampusResourceBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusResourceBooking.Services;

public sealed class IdentityService(CampusResourceBookingContext db, PasswordHashingService passwordHashingService)
{
    public async Task<ServiceResult<CampusUser>> SignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await db.CampusUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Email == normalizedEmail && item.IsActive, cancellationToken);

        if (user is null || !passwordHashingService.Verify(password, user.PasswordHash))
        {
            return ServiceResult<CampusUser>.Failure("Nieprawidłowy email lub hasło.");
        }

        return ServiceResult<CampusUser>.Success(user, "Zalogowano pomyślnie.");
    }
}
