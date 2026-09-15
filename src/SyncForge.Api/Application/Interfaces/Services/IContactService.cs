using SyncForge.Api.Application.DTOs.Requests;
using SyncForge.Api.Application.DTOs.Responses;

namespace SyncForge.Api.Application.Interfaces.Services;

public interface IContactService
{
    Task<ContactResponse> CreateAsync(
        CreateContactRequest request,
        CancellationToken cancellationToken = default
    );
}