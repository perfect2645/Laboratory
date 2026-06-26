using Messaging.Http.Policies;
using Microsoft.Net.Http.Headers;

namespace Messaging.Http.Client
{
    public record HttpApiClientOptions : IHttpApiClientOptions
    {
        public required string ApiKey { get; set; }
        public required Uri BaseAddress { get; set; }
        public string? Resource { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        public Dictionary<string, string> DefaultHeaders { get; set; } = new()
        {
            { HeaderNames.Accept, "application/json" },
            { HeaderNames.UserAgent, "HttpApiClient" }
        };

        public IHttpClientPolicy? Policy { get; set; }
    }
}
