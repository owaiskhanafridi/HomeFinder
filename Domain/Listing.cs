using System.ComponentModel.DataAnnotations;

public sealed class Listing
{
    public Guid Id { get; set; } = Guid.NewGuid();

    //default! assings the default value of string (which is null) but uses 
    // the null-forgiving operator ! to silence the compiler warning. 
    [Required, MaxLength(120)]
    public string Title { get; set; } = default!;
    [MaxLength(2000)]
    public string? Description { get; set; } = default!;

    [Required, MaxLength(200)]
    public string AddressLine1 { get; set; } = default!;
    [Required, MaxLength(200)]
    public string? AddressLine2 { get; set; } = default!;
    
    [Required, MaxLength(80)]
    public string City { get; set; } = default!;

    [Required, MaxLength(40)]
    public string State { get; set; } = default!;

    [Required, MaxLength(20)]
    public string ZipCode { get; set; } = default!;

    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public decimal Bathrooms { get; set; }
    public int SquareFeet { get; set; }

    public double? Longitude { get; set; }
    public double? Latitude { get; set; }

    [Required, MaxLength(100)]
    public string OwnerName { get; set; } = default!;
    [Required, MaxLength(100), EmailAddress]
    public string OwnerEmail { get; set; } = default!;

    [MaxLength(20)]
    public string? OwnerPhone { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}