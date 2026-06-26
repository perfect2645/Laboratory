namespace Messaging.Http.Policies;

public interface IRetryPolicy : IHttpClientPolicy
{
    public bool EnableRetry { get; set; }
    public int MaxRetryCount { get; set; }
    public int RetryDelay { get; set; }
}