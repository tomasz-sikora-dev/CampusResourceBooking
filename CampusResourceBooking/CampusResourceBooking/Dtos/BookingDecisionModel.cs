using CampusResourceBooking.Models;
using System.ComponentModel.DataAnnotations;

namespace CampusResourceBooking.Dtos;

public sealed class BookingDecisionModel
{
    public int BookingId { get; set; }
    public BookingStatus Status { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }
}
