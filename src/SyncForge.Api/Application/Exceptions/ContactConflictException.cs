namespace SyncForge.Api.Application.Exceptions;

public sealed class ContactConflictException : Exception
{
    public ContactConflictException(string message)
        : base(message)
    {
    }

    public ContactConflictException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}