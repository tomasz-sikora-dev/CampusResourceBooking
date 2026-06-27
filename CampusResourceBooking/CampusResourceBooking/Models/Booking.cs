using System.ComponentModel.DataAnnotations;

namespace CampusResourceBooking.Models;

public sealed class Booking
{
    public int BookingId { get; set; }

    public int ResourceId { get; set; }
    public Resource? Resource { get; set; }

    public int RequestedByUserId { get; set; }
    public CampusUser? RequestedBy { get; set; }

    public int? DecidedByUserId { get; set; }
    public CampusUser? DecidedBy { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    [Required, MaxLength(200)]
    public string Purpose { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    [MaxLength(1000)]
    public string? DecisionComment { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Submitted;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DecidedAt { get; set; }

    public bool IsPending => Status == BookingStatus.Submitted;
}
