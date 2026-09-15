using SyncForge.Api.Domain.Entities;
using SyncForge.Api.Domain.ValueObjects;

namespace SyncForge.Api.Application.Interfaces.Persistence;

public interface IContactRepository
{
    Task<bool> ExistsByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(
        Contact contact,
        CancellationToken cancellationToken = default
    );
}