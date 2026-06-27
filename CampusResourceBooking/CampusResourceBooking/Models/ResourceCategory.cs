using System.ComponentModel.DataAnnotations;

namespace CampusResourceBooking.Models;

public sealed class ResourceCategory
{
    public int ResourceCategoryId { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Description { get; set; }

    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
