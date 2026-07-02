using System.Collections.ObjectModel;
using Messaging.Http.Exceptions;

namespace MessagingTest.Http.CustomHttpClientSample.Policy;

public class CustomRetryInfo
{
    public int Attempt { get; init; }
    public required string RequestUrl { get; init; }
    public HttpException? HttpException { get; set; }
    public ReadOnlyDictionary<string, string>? RequestHeaders { get; init; }
}