using System.Collections.ObjectModel;
using System.Net;
using Messaging.Http.Client;
using Messaging.Http.Policies;

namespace MessagingTest.Http.CustomHttpClientSample.Policy;

public class CustomHttpRequestRetryHandler : DelegatingHandler
{
    private readonly IRetryPolicy _retryPolicy;
    private const int TooManyRequests = 429;
    private readonly TimeSpan _maxRetryDelay = TimeSpan.FromSeconds(30);

    protected Action<CustomRetryInfo>? RetryStatusAction { get; set; }

    public CustomHttpRequestRetryHandler(IHttpApiClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var retryPolicy = options.Policy as RetryPolicy;
        ArgumentNullException.ThrowIfNull(retryPolicy);
        _retryPolicy = retryPolicy;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_retryPolicy.EnableRetry)
            return await base.SendAsync(request, cancellationToken);

        var maxRetries = _retryPolicy.MaxRetryCount;
        var baseDelay = TimeSpan.FromSeconds(_retryPolicy.RetryDelay);

        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                if (attempt == maxRetries || !IsNeedRetryByStatusCode(response.StatusCode))
                    return response;
                // The HttpRequestMessage can only be sent once, so we need to clone it for the retry.
                await Task.Delay(CalculateExponentialBackoffDelay(baseDelay, attempt + 1), cancellationToken);
                request = await CloneHttpRequestMessage(request);
            }
            catch (HttpRequestException ex)
            {
                FireRetryStatusAction(request, attempt, ex);
                // throw after last try
                if (attempt == maxRetries)
                    throw;

                await Task.Delay(CalculateExponentialBackoffDelay(baseDelay, attempt + 1), cancellationToken);
                request = await CloneHttpRequestMessage(request);
            }
        }

        return null;
    }

    private void FireRetryStatusAction(HttpRequestMessage request, int attempt, HttpRequestException ex)
    {
        if (RetryStatusAction == null)
        {
            return;
        }

        var retryInfo = new CustomRetryInfo
        {
            Attempt = attempt,
            RequestUrl = request.RequestUri!.ToString(),
            HttpRequestException = ex,
            RequestHeaders = new ReadOnlyDictionary<string, string>(
                request.Headers.ToDictionary(h => h.Key, h => string.Join(",", h.Value)))
        };
        RetryStatusAction.Invoke(retryInfo);
    }

    private TimeSpan CalculateExponentialBackoffDelay(TimeSpan baseDelay, int retryCount)
    {
        double delayMs = baseDelay.TotalMilliseconds * Math.Pow(2, retryCount);
        delayMs = Math.Min(delayMs, _maxRetryDelay.TotalMilliseconds);
        return TimeSpan.FromMilliseconds(delayMs);
    }

    private bool IsNeedRetryByStatusCode(HttpStatusCode statusCode)
    {
        int code = (int)statusCode;

        // 5xx server error
        // 429 too many requests (server side limits the bandwidth)
        return code >= 500 || code == TooManyRequests;
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessage(HttpRequestMessage source)
    {
        var clone = new HttpRequestMessage(source.Method, source.RequestUri);
        foreach (var header in source.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (source.Content != null)
        {
            var stream = await source.Content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var header in source.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return clone;
    }
}
