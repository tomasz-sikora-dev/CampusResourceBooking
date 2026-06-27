namespace CampusResourceBooking.Models;

public sealed class VBookingFull
{
    public int BookingId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? DecisionComment { get; set; }
    public DateTime CreatedAt { get; set; }
}
