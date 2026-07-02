using Messaging.Http.Client;
using Messaging.Http.Exceptions;
using Microsoft.Extensions.Configuration;
using Utils.Configuration;

namespace Messaging.Http.Configurations;

public static class HttpConfigHelper
{
    extension(HttpApiClientConfig config)
    {
        public HttpApiClientOptions ToApiClientOptions()
        {
            try
            {
                var runtimeHeaders = new Dictionary<string, string>();
                if (config.DefaultHeaders is { Count: > 0 })
                {
                    foreach (var (key, value) in config.DefaultHeaders)
                    {
                        runtimeHeaders[key] = value;
                    }
                }

                return new HttpApiClientOptions
                {
                    ApiKey = config.ApiKey,
                    Resource = config.Resource,
                    BaseAddress = config.GetFullBaseAddress(),
                    Timeout = TimeSpan.FromSeconds(config.Timeout),
                    DefaultHeaders = runtimeHeaders,
                    Policy = config.RetryPolicy
                };
            }
            catch (Exception ex)
            {
                throw new HttpException(ex, "Convert http settings (in appsettings.json) to HttpApiClientOptions failed.", HttpStatus.Configuration );
            }
        }
    }
    
    public static HttpApiClientConfig? ReadFromConfig(string section, string apiKey)
    {
        try
        {
            var httpSettings = AppConfig.Configuration!.GetSection(section)
                .Get<IReadOnlyList<HttpApiClientConfig>>();

            if (httpSettings == null)
            {
                throw new HttpException($"Don't find section=[{section}] in appsettings.", HttpStatus.Configuration );
            }
            
            return httpSettings.FirstOrDefault(x => x.ApiKey == apiKey);
        }
        catch (Exception ex)
        {
            throw new HttpException(ex, $"Failed to get section=[{section}], apiKey=[{apiKey}] from appsettings", HttpStatus.Configuration );
        }
    }
}