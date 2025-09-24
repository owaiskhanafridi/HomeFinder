

using HomeFinder.Api.Features.Listings.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public sealed class ListingsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ListingsController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpPost]
    public async Task<ActionResult<Listing>> Create([FromBody] ListingCreateRequest req, CancellationToken ct)
    {
        var entity = new Listing
        {
            Title = req.Title,
            Description = req.Description,
            AddressLine1 = req.AddressLine1,
            AddressLine2 = req.AddressLine2,
            City = req.City,
            State = req.State,
            ZipCode = req.Zip,
            Price = req.Price,
            Bedrooms = req.Bedrooms,
            Bathrooms = req.Bathrooms,
            SquareFeet = req.SquareFeet,
            Latitude = req.Latitude,
            Longitude = req.Longitude,
            OwnerName = req.OwnerName,
            OwnerEmail = req.OwnerEmail,
            OwnerPhone = req.OwnerPhone
        };

        //Check for similar name and return an 409 conflict error.
        //TODO: Check how these errors and message can be handled from a single location.
        //Removing the following code will generate a 500 Error and 
        // will show user technical exception in response
        var exists = await _db.Listings
    .AnyAsync(l => l.Title == req.Title); // or compare normalized
        if (exists) return Conflict("A listing with this title already exists.");

        _db.Listings.Add(entity);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ListingDetail>> GetById(Guid id, CancellationToken ct)
    {
        var l = await _db.Listings.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (l is null) return NotFound();

        return new ListingDetail(
            l.Id, l.Title, l.Description, l.AddressLine1, l.AddressLine2, l.City, l.State, l.ZipCode,
            l.Price, l.Bedrooms, l.Bathrooms, l.SquareFeet, l.Latitude, l.Longitude,
            l.OwnerName, l.OwnerEmail, l.OwnerPhone, l.CreatedAt, l.IsActive,
            l.Photos.OrderByDescending(p => p.IsPrimary).ThenBy(p => p.UploadedAt).Select(p => p.Url).ToList());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListingSummary>>> Search(
    [FromQuery] string? q, [FromQuery] string? city, [FromQuery] string? state,
    [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
    [FromQuery] int? beds, [FromQuery] decimal? baths,
    [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
    CancellationToken ct = default)
    {
        var query = _db.Listings.AsNoTracking().Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Title.Contains(q) || (x.Description != null && x.Description.Contains(q)));
        if (!string.IsNullOrWhiteSpace(city)) query = query.Where(x => x.City == city);
        if (!string.IsNullOrWhiteSpace(state)) query = query.Where(x => x.State == state);
        if (minPrice.HasValue) query = query.Where(x => x.Price >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(x => x.Price <= maxPrice.Value);
        if (beds.HasValue) query = query.Where(x => x.Bedrooms >= beds.Value);
        if (baths.HasValue) query = query.Where(x => x.Bathrooms >= baths.Value);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ListingSummary(
                x.Id, x.Title, x.City, x.State, x.Price, x.Bedrooms, x.Bathrooms, x.SquareFeet,
                x.Photos.OrderByDescending(p => p.IsPrimary).Select(p => p.Url).FirstOrDefault()))
            .ToListAsync(ct);

        return items;
    }

    [HttpPost("{id:guid}/photos")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult> UploadPhoto(Guid id, IFormFile file, [FromQuery] bool isPrimary = false, CancellationToken ct = default)
    {
        var listing = await _db.Listings.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (listing is null) return NotFound("Listing not found");
        if (file is null || file.Length == 0) return BadRequest("No file");

        var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", id.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsRoot, fileName);
        await using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream, ct);

        var url = $"/uploads/{id}/{fileName}";
        var photo = new Photo { ListingId = id, Url = url, IsPrimary = isPrimary };
        if (isPrimary)
        {
            var existing = _db.Photos.Where(p => p.ListingId == id && p.IsPrimary);
            await existing.ForEachAsync(p => p.IsPrimary = false, ct);
        }
        _db.Photos.Add(photo);
        await _db.SaveChangesAsync(ct);

        return Created(url, new { url });
    }

    // Tour request
    [HttpPost("{id:guid}/tour-requests")]
    public async Task<ActionResult<Guid>> CreateTour(Guid id, [FromBody] CreateTourRequest req, CancellationToken ct)
    {
        var exists = await _db.Listings.AnyAsync(x => x.Id == id, ct);
        if (!exists) return NotFound("Listing not found");

        var tr = new TourRequest
        {
            ListingId = id,
            Name = req.Name,
            Email = req.Email,
            Phone = req.Phone,
            PreferredDateTime = req.PreferredDateTime,
            Message = req.Message
        };
        _db.TourRequests.Add(tr);
        await _db.SaveChangesAsync(ct);
        return Created($"/api/listings/{id}/tour-requests/{tr.Id}", tr.Id);
    }

    // Contact request
    [HttpPost("{id:guid}/contact-requests")]
    public async Task<ActionResult<Guid>> CreateContact(Guid id, [FromBody] CreateContactRequest req, CancellationToken ct)
    {
        var exists = await _db.Listings.AnyAsync(x => x.Id == id, ct);
        if (!exists) return NotFound("Listing not found");

        var cr = new ContactRequest { ListingId = id, Name = req.Name, Email = req.Email, Phone = req.Phone, Message = req.Message };
        _db.ContactRequests.Add(cr);
        await _db.SaveChangesAsync(ct);
        return Created($"/api/listings/{id}/contact-requests/{cr.Id}", cr.Id);
    }
}