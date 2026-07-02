using System.Text;
using Messaging.Http.Client;

namespace MessagingTest.Http.CustomHttpClientSample.Policy;

public class CustomHttpRequestRetryHandlerPlus : CustomHttpRequestRetryHandler
{
    public CustomHttpRequestRetryHandlerPlus(IHttpApiClientOptions options) : base(options)
    {
        RetryStatusAction += LogRetryStatus;
    }

    private void LogRetryStatus(CustomRetryInfo retryInfo)
    {
        var sb = new StringBuilder();
        
        var fullErrorMessages = retryInfo.HttpException?.GetDetailedMessages();

        if (string.IsNullOrEmpty(fullErrorMessages))
        {
            sb.AppendLine(fullErrorMessages);
        }

        sb.AppendLine(retryInfo.Attempt == 0
            ? $"Attempt to retry [{retryInfo.RequestUrl}]."
            : $"Retried request[{retryInfo.RequestUrl}], attempt={retryInfo.Attempt}");

        if (retryInfo.RequestHeaders is { Count: > 0 })
        {
            foreach (var header in retryInfo.RequestHeaders)
            {
                sb.AppendLine($"attempt[{retryInfo.Attempt}] {header.Key}:{header.Value}");
            }
        }
        
        Console.WriteLine(sb.ToString());
    }
}