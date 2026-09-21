using SyncForge.Api.Domain.Entities;
using SyncForge.Api.Infrastructure.Data.MongoDB.Documents;

namespace SyncForge.Api.Infrastructure.Data.MongoDB.Mappers;

public static class ContactDocumentMapper
{
    public static ContactDocument ToDocument(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        return new ContactDocument
        {
            Id = contact.Id,
            Email = contact.Email.Value,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Phone = contact.Phone,
            SyncStatus = contact.SyncStatus,
            HubSpotContactId = contact.HubSpotContactId,
            LastSyncError = contact.LastSyncError,
            SyncAttemptCount = contact.SyncAttemptCount,
            CreatedAt = contact.CreatedAt.UtcDateTime,
            UpdatedAt = contact.UpdatedAt.UtcDateTime,
            LastSyncedAt = contact.LastSyncedAt?.UtcDateTime
        };
    }
}