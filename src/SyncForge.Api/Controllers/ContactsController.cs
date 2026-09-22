using Microsoft.AspNetCore.Mvc;
using SyncForge.Api.Application.DTOs.Requests;
using SyncForge.Api.Application.DTOs.Responses;
using SyncForge.Api.Application.Interfaces.Services;

namespace SyncForge.Api.Controllers;

[ApiController]
[Route("api/contacts")]
public sealed class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContactResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContactResponse>> CreateAsync(
        [FromBody] CreateContactRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _contactService.CreateAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }
}