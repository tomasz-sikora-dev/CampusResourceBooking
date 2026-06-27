using CampusResourceBooking.Models;

namespace CampusResourceBooking.Dtos;

public sealed class BookingFilter
{
    public int? RequesterId { get; set; }
    public int? ResourceId { get; set; }
    public BookingStatus? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
