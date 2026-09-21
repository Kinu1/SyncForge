using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SyncForge.Api.Domain.Enums;

namespace SyncForge.Api.Infrastructure.Data.MongoDB.Documents;

public sealed class ContactDocument
{
    public const string CollectionName = "contacts";

    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("firstName")]
    public string? FirstName { get; set; }

    [BsonElement("lastName")]
    public string? LastName { get; set; }

    [BsonElement("phone")]
    public string? Phone { get; set; }

    [BsonElement("syncStatus")]
    [BsonRepresentation(BsonType.String)]
    public ContactSyncStatus SyncStatus { get; set; }

    [BsonElement("hubSpotContactId")]
    public string? HubSpotContactId { get; set; }

    [BsonElement("lastSyncError")]
    public string? LastSyncError { get; set; }

    [BsonElement("syncAttemptCount")]
    public int SyncAttemptCount { get; set; }

    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("lastSyncedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastSyncedAt { get; set; }
}