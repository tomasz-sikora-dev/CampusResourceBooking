using System.ComponentModel.DataAnnotations;

namespace CampusResourceBooking.Dtos;

public sealed class BookingFormModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Wybierz zasób.")]
    public int ResourceId { get; set; }

    [Required(ErrorMessage = "Podaj cel rezerwacji.")]
    [StringLength(200, ErrorMessage = "Cel rezerwacji może mieć maksymalnie 200 znaków.")]
    public string Purpose { get; set; } = string.Empty;

    public DateTime? Date { get; set; } = DateTime.Today.AddDays(1);

    [Required]
    public TimeSpan? StartsAt { get; set; } = new TimeSpan(9, 0, 0);

    [Required]
    public TimeSpan? EndsAt { get; set; } = new TimeSpan(10, 0, 0);

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime StartDateTime => Date.GetValueOrDefault(DateTime.Today).Date.Add(StartsAt.GetValueOrDefault(new TimeSpan(9, 0, 0)));
    public DateTime EndDateTime => Date.GetValueOrDefault(DateTime.Today).Date.Add(EndsAt.GetValueOrDefault(new TimeSpan(10, 0, 0)));
}
