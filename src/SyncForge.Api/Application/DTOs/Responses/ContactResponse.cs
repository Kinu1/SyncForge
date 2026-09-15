namespace SyncForge.Api.Application.DTOs.Responses;

public sealed record ContactResponse(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string SyncStatus,
    string? HubSpotContactId,
    int SyncAttemptCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdateAt,
    DateTimeOffset? LastSyncedAt
);