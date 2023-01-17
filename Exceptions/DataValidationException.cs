namespace SafeShare.Exceptions;

/// <summary>
///     The exception that is thrown when the a domain model has invalid data.
/// </summary>
public class DataValidationException : Exception
{
    public DataValidationException(string field, string message) : base(message)
    {
        Field = field;
    }

    public string Field { get; }
}