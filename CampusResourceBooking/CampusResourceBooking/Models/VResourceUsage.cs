namespace CampusResourceBooking.Models;

public sealed class VResourceUsage
{
    public int ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int ApprovedBookings { get; set; }
    public int PendingBookings { get; set; }
    public DateTime? NearestBooking { get; set; }
}
