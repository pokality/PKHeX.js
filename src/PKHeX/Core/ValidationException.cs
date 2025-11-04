namespace PKHeX.Core;

public class ValidationException : Exception
{
    public string? Code { get; }

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, string code) : base(message)
    {
        Code = code;
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
