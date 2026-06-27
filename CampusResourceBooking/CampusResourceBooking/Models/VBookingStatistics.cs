namespace CampusResourceBooking.Models;

public sealed class VBookingStatistics
{
    public string Status { get; set; } = string.Empty;
    public int TotalBookings { get; set; }
    public int UniqueResources { get; set; }
}
