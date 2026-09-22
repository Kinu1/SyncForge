using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SyncForge.Api.Application.Exceptions;
using SyncForge.Api.Domain.Exceptions;

namespace SyncForge.Api.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            DomainValidationException => (
                StatusCodes.Status400BadRequest,
                "Dados inválidos",
                exception.Message),

            ContactConflictException => (
                StatusCodes.Status409Conflict,
                "Conflito no cadastro",
                exception.Message),

            BadHttpRequestException badRequestException => (
                badRequestException.StatusCode,
                "Requisição HTTP rejeitada",
                "Não foi possível processar o formato ou o tamanho da requisição."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro interno",
                "Ocorreu uma falha ao processar a solicitação.")
        };

        var traceId = Activity.Current?.Id
            ?? httpContext.TraceIdentifier;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Falha ao processar a requisição. TraceId: {TraceId}",
                traceId);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path.Value
        };

        problemDetails.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = statusCode;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
    }
}