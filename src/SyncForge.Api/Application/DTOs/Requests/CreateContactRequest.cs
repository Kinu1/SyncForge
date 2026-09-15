namespace SyncForge.Api.Application.DTOs.Requests;

public sealed record CreateContactRequest(
    string? Email,
    string? FirstName,
    string? LastName,
    string? Phone
);