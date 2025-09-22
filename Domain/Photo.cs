using System.ComponentModel.DataAnnotations;

public sealed class Photo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = default!;

    public string Url { get; set; } = default!;
    public bool IsPrimary { get; set; }
    public DateTime UploadAt { get; set; } = DateTime.UtcNow;
}