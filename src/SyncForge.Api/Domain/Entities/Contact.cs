using SyncForge.Api.Domain.Enums;
using SyncForge.Api.Domain.Exceptions;
using SyncForge.Api.Domain.ValueObjects;

namespace SyncForge.Api.Domain.Entities;

public sealed class Contact
{
    private const int MaxNameLength = 100;
    private const int MaxPhoneLength = 30;
    private const int MaxHubSpotIdLength = 100;
    private const int MaxSyncErrorLength = 1_000;

    private Contact(
        Guid id,
        Email email,
        string? firstName,
        string? lastName,
        string? phone,
        DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        SyncStatus = ContactSyncStatus.Pending;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public Guid Id { get; }

    public Email Email { get; private set; }

    public string? FirstName { get; private set; }

    public string? LastName { get; private set; }

    public string? Phone { get; private set; }

    public ContactSyncStatus SyncStatus { get; private set; }

    public string? HubSpotContactId { get; private set; }

    public string? LastSyncError { get; private set; }

    public int SyncAttemptCount { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? LastSyncedAt { get; private set; }

    public static Contact Create(
        Email email,
        string? firstName,
        string? lastName,
        string? phone,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(email);

        var normalizedCreatedAt = NormalizeTimestamp(createdAt);

        var normalizedFirstName = NormalizeOptionalText(
            firstName,
            "O primeiro nome",
            MaxNameLength);

        var normalizedLastName = NormalizeOptionalText(
            lastName,
            "O sobrenome",
            MaxNameLength);

        var normalizedPhone = NormalizeOptionalText(
            phone,
            "O telefone",
            MaxPhoneLength);

        return new Contact(
            Guid.NewGuid(),
            email,
            normalizedFirstName,
            normalizedLastName,
            normalizedPhone,
            normalizedCreatedAt);
    }

    public void UpdateDetails(
        Email email,
        string? firstName,
        string? lastName,
        string? phone,
        DateTimeOffset updatedAt)
    {
        ArgumentNullException.ThrowIfNull(email);

        if (SyncStatus == ContactSyncStatus.Processing)
        {
            throw new DomainValidationException(
                "Não é possível alterar um contato durante a sincronização.");
        }

        var normalizedUpdatedAt = NormalizeOccurrenceTimestamp(updatedAt);

        var normalizedFirstName = NormalizeOptionalText(
            firstName,
            "O primeiro nome",
            MaxNameLength);

        var normalizedLastName = NormalizeOptionalText(
            lastName,
            "O sobrenome",
            MaxNameLength);

        var normalizedPhone = NormalizeOptionalText(
            phone,
            "O telefone",
            MaxPhoneLength);

        Email = email;
        FirstName = normalizedFirstName;
        LastName = normalizedLastName;
        Phone = normalizedPhone;
        SyncStatus = ContactSyncStatus.Pending;
        LastSyncError = null;
        UpdatedAt = normalizedUpdatedAt;
    }

    public void MarkAsProcessing(DateTimeOffset processingStartedAt)
    {
        if (SyncStatus != ContactSyncStatus.Pending &&
            SyncStatus != ContactSyncStatus.Failed)
        {
            throw new DomainValidationException(
                "Somente contatos pendentes ou com falha podem iniciar uma sincronização.");
        }

        var normalizedTimestamp =
            NormalizeOccurrenceTimestamp(processingStartedAt);

        SyncStatus = ContactSyncStatus.Processing;
        SyncAttemptCount++;
        LastSyncError = null;
        UpdatedAt = normalizedTimestamp;
    }

    public void MarkAsSynchronized(
        string? hubSpotContactId,
        DateTimeOffset synchronizedAt)
    {
        if (SyncStatus != ContactSyncStatus.Processing)
        {
            throw new DomainValidationException(
                "A sincronização precisa estar em processamento antes de ser concluída.");
        }

        var normalizedHubSpotContactId = NormalizeRequiredText(
            hubSpotContactId,
            "O identificador do contato no HubSpot",
            MaxHubSpotIdLength);

        var normalizedTimestamp =
            NormalizeOccurrenceTimestamp(synchronizedAt);

        HubSpotContactId = normalizedHubSpotContactId;
        SyncStatus = ContactSyncStatus.Synchronized;
        LastSyncError = null;
        LastSyncedAt = normalizedTimestamp;
        UpdatedAt = normalizedTimestamp;
    }

    public void MarkAsFailed(
        string? errorMessage,
        DateTimeOffset failedAt)
    {
        if (SyncStatus != ContactSyncStatus.Processing)
        {
            throw new DomainValidationException(
                "Somente uma sincronização em processamento pode ser marcada como falha.");
        }

        var normalizedErrorMessage = NormalizeRequiredText(
            errorMessage,
            "A mensagem de erro da sincronização",
            MaxSyncErrorLength);

        var normalizedTimestamp =
            NormalizeOccurrenceTimestamp(failedAt);

        SyncStatus = ContactSyncStatus.Failed;
        LastSyncError = normalizedErrorMessage;
        UpdatedAt = normalizedTimestamp;
    }

    private static string? NormalizeOptionalText(
        string? value,
        string fieldName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maxLength)
        {
            throw new DomainValidationException(
                $"{fieldName} deve ter no máximo {maxLength} caracteres.");
        }

        return normalizedValue;
    }

    private static string NormalizeRequiredText(
        string? value,
        string fieldName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException(
                $"{fieldName} é obrigatório.");
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maxLength)
        {
            throw new DomainValidationException(
                $"{fieldName} deve ter no máximo {maxLength} caracteres.");
        }

        return normalizedValue;
    }

    private static DateTimeOffset NormalizeTimestamp(
        DateTimeOffset timestamp)
    {
        if (timestamp == default)
        {
            throw new DomainValidationException(
                "A data da operação é obrigatória.");
        }

        return timestamp.ToUniversalTime();
    }

    private DateTimeOffset NormalizeOccurrenceTimestamp(
        DateTimeOffset timestamp)
    {
        var normalizedTimestamp = NormalizeTimestamp(timestamp);

        if (normalizedTimestamp < UpdatedAt)
        {
            throw new DomainValidationException(
                "A data da operação não pode ser anterior à última atualização do contato.");
        }

        return normalizedTimestamp;
    }
}