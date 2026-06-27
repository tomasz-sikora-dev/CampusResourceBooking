using CampusResourceBooking.Models;
using MudBlazor;

namespace CampusResourceBooking.Services;

public sealed record StatusInfo(string Label, Color Color, string Icon);

public sealed class StatusInfoProvider
{
    public StatusInfo Get(BookingStatus status) => status switch
    {
        BookingStatus.Draft => new("Robocza", Color.Default, Icons.Material.Filled.EditNote),
        BookingStatus.Submitted => new("Oczekuje", Color.Warning, Icons.Material.Filled.HourglassTop),
        BookingStatus.Approved => new("Zatwierdzona", Color.Success, Icons.Material.Filled.CheckCircle),
        BookingStatus.Rejected => new("Odrzucona", Color.Error, Icons.Material.Filled.Cancel),
        BookingStatus.Cancelled => new("Anulowana", Color.Default, Icons.Material.Filled.Block),
        BookingStatus.Completed => new("Zakończona", Color.Info, Icons.Material.Filled.DoneAll),
        _ => new("Nieznany", Color.Default, Icons.Material.Filled.Help)
    };
}
