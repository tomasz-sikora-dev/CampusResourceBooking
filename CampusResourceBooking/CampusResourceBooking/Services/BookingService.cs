using CampusResourceBooking.Dtos;
using CampusResourceBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusResourceBooking.Services;

public sealed class BookingService(CampusResourceBookingContext db, BookingTimeValidator timeValidator)
{
    public async Task<List<Booking>> GetBookingsAsync(BookingFilter filter, CancellationToken cancellationToken = default)
    {
        var query = db.Bookings
            .Include(booking => booking.Resource)
            .ThenInclude(resource => resource!.Category)
            .Include(booking => booking.RequestedBy)
            .AsNoTracking()
            .AsQueryable();

        if (filter.RequesterId.HasValue)
        {
            query = query.Where(booking => booking.RequestedByUserId == filter.RequesterId.Value);
        }

        if (filter.ResourceId.HasValue)
        {
            query = query.Where(booking => booking.ResourceId == filter.ResourceId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(booking => booking.Status == filter.Status.Value);
        }

        if (filter.From.HasValue)
        {
            query = query.Where(booking => booking.StartsAt >= filter.From.Value);
        }

        if (filter.To.HasValue)
        {
            query = query.Where(booking => booking.StartsAt <= filter.To.Value);
        }

        return await query
            .OrderByDescending(booking => booking.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Booking?> GetBookingAsync(int bookingId, CancellationToken cancellationToken = default) =>
        await db.Bookings
            .Include(booking => booking.Resource)
            .ThenInclude(resource => resource!.Category)
            .Include(booking => booking.RequestedBy)
            .Include(booking => booking.DecidedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(booking => booking.BookingId == bookingId, cancellationToken);

    public async Task<ServiceResult<int>> CreateBookingAsync(int requesterId, BookingFormModel model, CancellationToken cancellationToken = default)
    {
        if (model.Date is null || model.StartsAt is null || model.EndsAt is null)
        {
            return ServiceResult<int>.Failure("Podaj datę oraz godziny rezerwacji.");
        }

        var validationErrors = timeValidator.Validate(model.StartDateTime, model.EndDateTime, DateTime.Now);
        if (validationErrors.Count > 0)
        {
            return ServiceResult<int>.Failure(string.Join(" ", validationErrors));
        }

        var resource = await db.Resources.AsNoTracking()
            .FirstOrDefaultAsync(item => item.ResourceId == model.ResourceId && item.IsActive, cancellationToken);

        if (resource is null)
        {
            return ServiceResult<int>.Failure("Wybrany zasób jest niedostępny.");
        }

        var hasCollision = await HasCollisionAsync(model.ResourceId, model.StartDateTime, model.EndDateTime, null, cancellationToken);
        if (hasCollision)
        {
            return ServiceResult<int>.Failure("Zasób jest już zarezerwowany w wybranym terminie.");
        }

        var booking = new Booking
        {
            ResourceId = model.ResourceId,
            RequestedByUserId = requesterId,
            StartsAt = model.StartDateTime,
            EndsAt = model.EndDateTime,
            Purpose = model.Purpose.Trim(),
            Notes = model.Notes?.Trim(),
            Status = resource.RequiresApproval ? BookingStatus.Submitted : BookingStatus.Approved,
            CreatedAt = DateTime.UtcNow
        };

        db.Bookings.Add(booking);
        await db.SaveChangesAsync(cancellationToken);

        return ServiceResult<int>.Success(booking.BookingId, "Rezerwacja została utworzona.");
    }

    public async Task<ServiceResult> DecideAsync(BookingDecisionModel decision, int adminUserId, CancellationToken cancellationToken = default)
    {
        if (decision.Status is not (BookingStatus.Approved or BookingStatus.Rejected))
        {
            return ServiceResult.Failure("Decyzja musi zatwierdzać albo odrzucać rezerwację.");
        }

        var booking = await db.Bookings.FirstOrDefaultAsync(item => item.BookingId == decision.BookingId, cancellationToken);
        if (booking is null)
        {
            return ServiceResult.Failure("Nie znaleziono rezerwacji.");
        }

        if (booking.Status != BookingStatus.Submitted)
        {
            return ServiceResult.Failure("Decyzję można podjąć tylko dla rezerwacji oczekujących.");
        }

        if (decision.Status == BookingStatus.Approved)
        {
            var hasCollision = await HasCollisionAsync(booking.ResourceId, booking.StartsAt, booking.EndsAt, booking.BookingId, cancellationToken);
            if (hasCollision)
            {
                return ServiceResult.Failure("Nie można zatwierdzić rezerwacji, ponieważ termin koliduje z inną zatwierdzoną rezerwacją.");
            }
        }

        booking.Status = decision.Status;
        booking.DecisionComment = decision.Comment?.Trim();
        booking.DecidedByUserId = adminUserId;
        booking.DecidedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success("Status rezerwacji został zmieniony.");
    }

    public async Task<ServiceResult> CancelAsync(int bookingId, int requesterId, CancellationToken cancellationToken = default)
    {
        var booking = await db.Bookings.FirstOrDefaultAsync(item => item.BookingId == bookingId, cancellationToken);
        if (booking is null)
        {
            return ServiceResult.Failure("Nie znaleziono rezerwacji.");
        }

        if (booking.RequestedByUserId != requesterId)
        {
            return ServiceResult.Failure("Możesz anulować tylko własną rezerwację.");
        }

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed or BookingStatus.Rejected)
        {
            return ServiceResult.Failure("Tej rezerwacji nie można już anulować.");
        }

        booking.Status = BookingStatus.Cancelled;
        await db.SaveChangesAsync(cancellationToken);
        return ServiceResult.Success("Rezerwacja została anulowana.");
    }

    private Task<bool> HasCollisionAsync(int resourceId, DateTime startsAt, DateTime endsAt, int? excludedBookingId, CancellationToken cancellationToken)
    {
        return db.Bookings.AnyAsync(booking =>
            booking.ResourceId == resourceId &&
            booking.Status == BookingStatus.Approved &&
            (!excludedBookingId.HasValue || booking.BookingId != excludedBookingId.Value) &&
            booking.StartsAt < endsAt &&
            startsAt < booking.EndsAt,
            cancellationToken);
    }
}
