namespace RealEstateApi.Application.Exceptions;

/// <summary>
/// Exception thrown when a request contains invalid data or violates business rules
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
