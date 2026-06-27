using System.ComponentModel.DataAnnotations;

namespace CampusResourceBooking.Models;

public sealed class CampusUser
{
    public int CampusUserId { get; set; }

    [Required, MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(128)]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    [MaxLength(80)]
    public string? StudentNumber { get; set; }

    [MaxLength(120)]
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public string FullName => $"{FirstName} {LastName}";
}
