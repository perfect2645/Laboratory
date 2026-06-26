using Messaging.Http.Policies;

namespace Messaging.Http.Client;

public interface IHttpApiClientOptions
{
    string ApiKey { get; set; }
    Uri BaseAddress { get; set; }
    string? Resource { get; set; }
    TimeSpan Timeout { get; set; }
    Dictionary<string, string> DefaultHeaders { get; set; }
    IHttpClientPolicy? Policy { get; set; }
}