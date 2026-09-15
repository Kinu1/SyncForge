using SyncForge.Api.Application.DTOs.Requests;
using SyncForge.Api.Application.DTOs.Responses;
using SyncForge.Api.Application.Interfaces.Persistence;
using SyncForge.Api.Application.Interfaces.Services;
using SyncForge.Api.Domain.Entities;
using SyncForge.Api.Domain.Exceptions;
using SyncForge.Api.Domain.ValueObjects;

namespace SyncForge.Api.Application.Services;

public sealed class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly TimeProvider _timeProvider;

    public ContactService(
        IContactRepository contactRepository,
        TimeProvider timeProvider)
    {
        _contactRepository = contactRepository;
        _timeProvider = timeProvider;
    }

    public async Task<ContactResponse> CreateAsync(
        CreateContactRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = Email.Create(request.Email);

        var contactAlreadyExists = await _contactRepository.ExistsByEmailAsync(
            email,
            cancellationToken);

        if (contactAlreadyExists)
        {
            throw new DomainValidationException(
                "Já existe um contato cadastrado com este e-mail.");
        }

        var contact = Contact.Create(
            email,
            request.FirstName,
            request.LastName,
            request.Phone,
            _timeProvider.GetUtcNow());

        await _contactRepository.AddAsync(
            contact,
            cancellationToken);

        return ToResponse(contact);
    }

    private static ContactResponse ToResponse(Contact contact)
    {
        return new ContactResponse(
            contact.Id,
            contact.Email.Value,
            contact.FirstName,
            contact.LastName,
            contact.Phone,
            contact.SyncStatus.ToString(),
            contact.HubSpotContactId,
            contact.SyncAttemptCount,
            contact.CreatedAt,
            contact.UpdatedAt,
            contact.LastSyncedAt);
    }
}