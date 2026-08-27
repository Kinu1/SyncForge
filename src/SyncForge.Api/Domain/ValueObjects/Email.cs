using System.Net.Mail;
using SyncForge.Api.Domain.Exceptions;

namespace SyncForge.Api.Domain.ValueObjects;

public sealed record Email
{
    private const int MaxLength = 254;

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException(
                "O e-mail é obrigatório.");
        }

        var normalizedValue = value.Trim().ToLowerInvariant();

        if (normalizedValue.Length > MaxLength)
        {
            throw new DomainValidationException(
                $"O e-mail deve ter no máximo {MaxLength} caracteres.");
        }

        var isValidEmail =
            MailAddress.TryCreate(normalizedValue, out var parsedEmail) &&
            string.Equals(
                parsedEmail.Address,
                normalizedValue,
                StringComparison.OrdinalIgnoreCase);

        if (!isValidEmail)
        {
            throw new DomainValidationException(
                "O formato do e-mail é inválido.");
        }

        return new Email(normalizedValue);
    }

    public override string ToString()
    {
        return Value;
    }
}