namespace DependencyRadar.Services;

public class OsvServiceException : Exception
{
    public OsvServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}