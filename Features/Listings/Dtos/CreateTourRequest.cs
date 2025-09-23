public record CreateTourRequest(
    string Name,
    string Email,
    string? Phone,
    DateTime PreferredDateTime,
    string? Message
    );
