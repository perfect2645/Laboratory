namespace Messaging.Http.Policies;

public class RetryPolicy : IRetryPolicy
{
    public bool EnableRetry { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public int RetryDelay { get; set; } = 1;
}