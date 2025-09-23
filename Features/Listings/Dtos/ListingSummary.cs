namespace HomeFinder.Api.Features.Listings.Dtos;

public sealed record ListingSummary(
    Guid Id,
    string Title,
    string City,
    string State,
    decimal Price,
    int Bedrooms,
    decimal Bathrooms,
    int SquareFeet,
    string? PrimaryPhotoUrl
);
