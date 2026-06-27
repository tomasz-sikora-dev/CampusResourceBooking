using CampusResourceBooking.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CampusResourceBooking.Services;

public sealed class BookingPdfService
{
    public byte[] GenerateConfirmation(Booking booking)
    {
        if (booking.Resource is null || booking.RequestedBy is null)
        {
            throw new InvalidOperationException("Booking must include resource and requester data.");
        }

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(text => text.FontSize(11));

                page.Header()
                    .Text("CampusResourceBooking - potwierdzenie rezerwacji")
                    .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(10);
                    column.Item().Text($"Numer rezerwacji: {booking.BookingId}").Bold();
                    column.Item().Text($"Zasób: {booking.Resource.Name}");
                    column.Item().Text($"Lokalizacja: {booking.Resource.Location}");
                    column.Item().Text($"Rezerwujący: {booking.RequestedBy.FullName} ({booking.RequestedBy.Email})");
                    column.Item().Text($"Termin: {booking.StartsAt:yyyy-MM-dd HH:mm} - {booking.EndsAt:HH:mm}");
                    column.Item().Text($"Cel: {booking.Purpose}");
                    column.Item().Text($"Status: {booking.Status}");

                    if (!string.IsNullOrWhiteSpace(booking.DecisionComment))
                    {
                        column.Item().Text($"Komentarz decyzji: {booking.DecisionComment}");
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Wygenerowano: ");
                        text.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    });
            });
        }).GeneratePdf();
    }
}
