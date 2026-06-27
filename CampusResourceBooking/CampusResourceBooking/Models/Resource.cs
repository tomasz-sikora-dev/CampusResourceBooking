using System.ComponentModel.DataAnnotations;

namespace CampusResourceBooking.Models;

public sealed class Resource
{
    public int ResourceId { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string Building { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string RoomNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool RequiresApproval { get; set; } = true;

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? Equipment { get; set; }

    public int ResourceCategoryId { get; set; }
    public ResourceCategory? Category { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public string Location => $"{Building}, {RoomNumber}";
}
