using System.ComponentModel.DataAnnotations;
using Messaging.Http.Policies;
using Microsoft.Net.Http.Headers;

namespace Messaging.Http.Configurations;

public class HttpApiClientConfig
{
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    
    [Required]
    public string BaseUrl { get; set; } = string.Empty;
    
    public string? Resource { get; set; }
    
    [Range(1, 6000, ErrorMessage = "Timeout must be between 1s and 6000s, default 10s.")]
    public int Timeout { get; set; }
    
    public RetryPolicy? RetryPolicy { get; set; }

    public Dictionary<string, string>? DefaultHeaders { get; set; } = new()
    {
        { HeaderNames.Accept, "application/json" },
        { HeaderNames.UserAgent, "HttpApiClient" }
    };

    public string GetFullBaseAddress()
    {
        if (string.IsNullOrWhiteSpace(Resource))
            return BaseUrl;
        var baseClean = BaseUrl.TrimEnd('/');
        var resourceClean = Resource.Trim('/');
        return $"{baseClean}/{resourceClean}/";
    }
}