namespace HomeFinder.Api.Features.Listings.Dtos;

public sealed record ListingCreateRequest(
    string Title,
    string? Description,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string Zip,
    decimal Price,
    int Bedrooms,
    decimal Bathrooms,
    int SquareFeet,
    double? Latitude,
    double? Longitude,
    string OwnerName,
    string OwnerEmail,
    string? OwnerPhone
);
