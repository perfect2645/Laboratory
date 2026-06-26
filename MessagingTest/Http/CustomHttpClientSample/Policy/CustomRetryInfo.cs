using System.Collections.ObjectModel;

namespace MessagingTest.Http.CustomHttpClientSample.Policy;

public class CustomRetryInfo
{
    public int Attempt { get; set; }
    public required string RequestUrl { get; set; }
    public HttpRequestException? HttpRequestException { get; set; }
    public ReadOnlyDictionary<string, string>? RequestHeaders { get; set; }
}