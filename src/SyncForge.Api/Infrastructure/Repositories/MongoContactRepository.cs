using MongoDB.Driver;
using SyncForge.Api.Application.Exceptions;
using SyncForge.Api.Application.Interfaces.Persistence;
using SyncForge.Api.Domain.Entities;
using SyncForge.Api.Domain.ValueObjects;
using SyncForge.Api.Infrastructure.Data.MongoDB.Documents;
using SyncForge.Api.Infrastructure.Data.MongoDB.Mappers;

namespace SyncForge.Api.Infrastructure.Repositories;

public sealed class MongoContactRepository : IContactRepository
{
    private readonly IMongoCollection<ContactDocument> _contacts;

    public MongoContactRepository(
        IMongoCollection<ContactDocument> contacts)
    {
        _contacts = contacts;
    }

    public Task<bool> ExistsByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);

        var filter = Builders<ContactDocument>.Filter.Eq(
            contact => contact.Email,
            email.Value);

        return _contacts
            .Find(filter)
            .AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Contact contact,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contact);

        var document = ContactDocumentMapper.ToDocument(contact);

        try
        {
            await _contacts.InsertOneAsync(
                document,
                cancellationToken: cancellationToken);
        }
        catch (MongoWriteException exception)
            when (exception.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new ContactConflictException(
                "Já existe um contato com este e-mail ou identificador.",
                exception);
        }
    }
}