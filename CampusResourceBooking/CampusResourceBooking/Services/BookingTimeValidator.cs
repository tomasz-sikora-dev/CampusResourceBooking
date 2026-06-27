namespace CampusResourceBooking.Services;

public sealed class BookingTimeValidator
{
    private static readonly TimeSpan EarliestBooking = new(7, 0, 0);
    private static readonly TimeSpan LatestBooking = new(22, 0, 0);
    private static readonly TimeSpan MinimumDuration = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan MaximumDuration = TimeSpan.FromHours(8);

    public IReadOnlyList<string> Validate(DateTime startsAt, DateTime endsAt, DateTime now)
    {
        var errors = new List<string>();

        if (startsAt <= now)
        {
            errors.Add("Rezerwacja musi rozpoczynać się w przyszłości.");
        }

        if (endsAt <= startsAt)
        {
            errors.Add("Godzina zakończenia musi być późniejsza niż godzina rozpoczęcia.");
            return errors;
        }

        var duration = endsAt - startsAt;
        if (duration < MinimumDuration)
        {
            errors.Add("Minimalny czas rezerwacji to 30 minut.");
        }

        if (duration > MaximumDuration)
        {
            errors.Add("Maksymalny czas rezerwacji to 8 godzin.");
        }

        if (startsAt.TimeOfDay < EarliestBooking || endsAt.TimeOfDay > LatestBooking)
        {
            errors.Add("Zasoby można rezerwować między 07:00 a 22:00.");
        }

        return errors;
    }
}
