using CampusResourceBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusResourceBooking.Services;

public sealed class ResourceService(CampusResourceBookingContext db)
{
    public Task<List<ResourceCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        db.ResourceCategories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);


    public Task<List<Resource>> GetResourcesAsync(CancellationToken cancellationToken = default) =>
        db.Resources
            .Include(resource => resource.Category)
            .AsNoTracking()
            .OrderBy(resource => resource.Building)
            .ThenBy(resource => resource.RoomNumber)
            .ToListAsync(cancellationToken);

    public Task<List<Resource>> GetActiveResourcesAsync(CancellationToken cancellationToken = default) =>
        db.Resources
            .Include(resource => resource.Category)
            .AsNoTracking()
            .Where(resource => resource.IsActive)
            .OrderBy(resource => resource.Building)
            .ThenBy(resource => resource.RoomNumber)
            .ToListAsync(cancellationToken);

    public Task<List<VResourceUsage>> GetUsageSummaryAsync(CancellationToken cancellationToken = default) =>
        db.VResourceUsage
            .AsNoTracking()
            .OrderByDescending(summary => summary.ApprovedBookings)
            .ThenBy(summary => summary.ResourceName)
            .ToListAsync(cancellationToken);

    public async Task<Resource?> GetResourceAsync(int resourceId, CancellationToken cancellationToken = default) =>
        await db.Resources
            .Include(resource => resource.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(resource => resource.ResourceId == resourceId, cancellationToken);

    public async Task AddResourceAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        db.Resources.Add(resource);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ToggleAvailabilityAsync(int resourceId, bool isActive, CancellationToken cancellationToken = default)
    {
        var resource = await db.Resources.FirstAsync(item => item.ResourceId == resourceId, cancellationToken);
        resource.IsActive = isActive;
        await db.SaveChangesAsync(cancellationToken);
    }
}
