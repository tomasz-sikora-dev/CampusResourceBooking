using CampusResourceBooking.Models;

namespace CampusResourceBooking.Services;

public sealed class AppState
{
    public int? UserId { get; private set; }
    public string? DisplayName { get; private set; }
    public string? Email { get; private set; }
    public UserRole? Role { get; private set; }

    public bool IsAuthenticated => UserId.HasValue;
    public bool IsAdministrator => Role == UserRole.Administrator;
    public bool IsStudent => Role == UserRole.Student;

    public event Action? StateChanged;

    public void SignIn(CampusUser user)
    {
        UserId = user.CampusUserId;
        DisplayName = user.FullName;
        Email = user.Email;
        Role = user.Role;
        NotifyStateChanged();
    }

    public void SignOut()
    {
        UserId = null;
        DisplayName = null;
        Email = null;
        Role = null;
        NotifyStateChanged();
    }

    public bool CanAccess(UserRole role) => IsAuthenticated && Role == role;

    private void NotifyStateChanged() => StateChanged?.Invoke();
}
