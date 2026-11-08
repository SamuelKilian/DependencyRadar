namespace DependencyRadar.Services;

public class OsvTimeoutException : Exception
{
    public OsvTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}