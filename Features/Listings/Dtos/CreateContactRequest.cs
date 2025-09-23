public record CreateContactRequest(
    string Name,
    string Email,
    string? Phone,
    string Message
    );
