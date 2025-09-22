using System.ComponentModel.DataAnnotations;


public enum TourStatus
{
    Pending,
    Confirmed,
    Declined
}
public sealed class TourRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = default!;

    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;
    [Required, MaxLength(200), EmailAddress]
    public string Email { get; set; } = default!;
    [MaxLength(20)]
    public string? Phone { get; set; }

    public DateTime PreferredDateTime { get; set; }
    [MaxLength(2000)]
    public string Message { get; set; } = default!;

    public TourStatus Status { get; set; } = TourStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}