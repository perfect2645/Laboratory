using Logging;
using Messaging.Http.Client;
using Messaging.Http.Configurations;
using Messaging.Http.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Messaging.Http.Ioc;

public static class ClientRegistrationExt
{
    #region IServiceCollection
    
    extension(IServiceCollection services)
    {
        public IHttpClientBuilder AddHttpApiClient(
            string clientIdentifier,
            Action<HttpApiClientOptions> configureOptions)
        {
            if (services == null)
            {
                throw new HttpException($"HttpClient Register error. Ioc Service :[{nameof(services)}] is null", HttpStatus.ClientRegister);
            }
            if (string.IsNullOrEmpty(clientIdentifier))
            {
                throw new HttpException($"HttpClient Register error. clientIdentifier is empty.", HttpStatus.ClientRegister);
            }
            if (configureOptions == null)
            {
                throw new HttpException($"HttpClient Register error. Client configureOptions is empty.", HttpStatus.ClientRegister);
            }

            services.Configure(clientIdentifier, configureOptions);

            var clientBuilder = services.AddHttpClient(clientIdentifier, (serviceProvider, httpClient) =>
            {
                BuildHttpClient(clientIdentifier, serviceProvider, httpClient);
            });
        
            return clientBuilder;
        }
        
        public IHttpClientBuilder AddHttpApiClient(
            string clientIdentifier,
            Func<HttpApiClientOptions> configureOptions)
        {
            if (services == null)
            {
                throw new HttpException($"HttpClient Register error. Ioc Service :[{nameof(services)}] is null", HttpStatus.ClientRegister);
            }
            if (string.IsNullOrEmpty(clientIdentifier))
            {
                throw new HttpException($"HttpClient Register error. clientIdentifier is empty.", HttpStatus.ClientRegister);
            }
            if (configureOptions == null)
            {
                throw new HttpException($"HttpClient Register error. Client configureOptions is empty.", HttpStatus.ClientRegister);
            }

            services.Configure<HttpApiClientOptions>(clientIdentifier, opt => opt.CopyAllFrom(configureOptions.Invoke()));

            var clientBuilder = services.AddHttpClient(clientIdentifier, (serviceProvider, httpClient) =>
            {
                BuildHttpClient(clientIdentifier, serviceProvider, httpClient);
            });
        
            return clientBuilder;
        }
        
        public IHttpClientBuilder AddHttpClientWithRetryPolicy(
            string clientIdentifier,
            Action<HttpApiClientOptions> configureOptions)
        {
            var clientBuilder = services.AddHttpApiClient(clientIdentifier, configureOptions)
                .AddPolicyHandler((serviceProvider, request) => BuildRetryPolicy(services, clientIdentifier, serviceProvider));

            return clientBuilder;
        }

        public IHttpClientBuilder AddHttpClientWithRetryPolicy(
            string clientIdentifier,
            Func<HttpApiClientOptions> configureOptions)
        {
            var clientBuilder = services.AddHttpApiClient(clientIdentifier, configureOptions)
                .AddPolicyHandler((serviceProvider, request) => BuildRetryPolicy(services, clientIdentifier, serviceProvider));

            return clientBuilder;
        }

        public IHttpClientBuilder AddConfiguratedHttpClient<TApiClient>(string apiKey)
            where TApiClient : HttpApiClient
        {
            var httpSettings = HttpConfigHelper.ReadFromConfig(HttpClientConstants.ApiSettingsHttp, apiKey);
            if (httpSettings is null)
            {
                throw new HttpException($"Read http client config [{apiKey}] failed. Please check your appsettings.json", HttpStatus.Configuration);
            }
            
            var builder = services.AddHttpApiClient(apiKey, httpSettings.ToApiClientOptions)
            .AddPolicyHandler((serviceProvider, _) => BuildRetryPolicy(services, apiKey, serviceProvider));
            
            services.AddKeyedTransient<TApiClient>(apiKey, (serviceProvider, _) =>
            {
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(apiKey);
                var clientInstance = Activator.CreateInstance(typeof(TApiClient), httpClient) as TApiClient
                       ?? throw new HttpException(
                           $"Failed to construct {typeof(TApiClient).Name}, must accept HttpClient as constructor parameter",
                           HttpStatus.ClientRegister);
                return clientInstance;
            });

            return builder;
        }
        
        private IAsyncPolicy<HttpResponseMessage> BuildRetryPolicy(string clientIdentifier, IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetClientOptions(clientIdentifier);
            if (options.Policy == null || options.Policy is not Policies.IRetryPolicy)
            {
                throw new HttpException($"HttpClient Retry Policy Registration error. IRetryPolicy not defined in client option.", HttpStatus.ClientRegister);
            }
                    
            var retryPolicy = options.Policy as Policies.IRetryPolicy;
                    
            if (!retryPolicy!.EnableRetry || retryPolicy.MaxRetryCount <= 0)
            {
                return Policy.NoOpAsync<HttpResponseMessage>();
            }

            return CreateRetryPolicy(retryPolicy.MaxRetryCount, TimeSpan.FromSeconds(retryPolicy.RetryDelay));
        }
        
        public void AddCustomHttpClient<TClient>(string clientIdentifier, 
            Func<IServiceProvider, HttpClient, IHttpApiClientOptions, TClient> customClient)
            where TClient : class
        {
            services.AddKeyedTransient<TClient>(clientIdentifier, (serviceProvider, client) =>
            {
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(clientIdentifier);
                var clientOptions = serviceProvider.GetClientOptions(clientIdentifier);
                return customClient.Invoke(serviceProvider, httpClient, clientOptions);
            });
        }
    }
    
    private static void BuildHttpClient(string clientIdentifier, IServiceProvider serviceProvider, HttpClient httpClient)
    {
        var options = serviceProvider.GetClientOptions(clientIdentifier);

        if (options.BaseAddress == null)
        {
            throw new HttpException("BaseAddress is required in http config", HttpStatus.Configuration);
        }

        httpClient.BaseAddress = options.BaseAddress;
        httpClient.Timeout = options.Timeout;
        foreach ((string key, string value) in options.DefaultHeaders)
        {
            if (!httpClient.DefaultRequestHeaders.TryAddWithoutValidation(key, value))
            {
                httpClient.DefaultRequestHeaders.Add(key, value);
            }
        }
    }
    
    private static AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy(int maxRetryCount, TimeSpan retryDelay)
    {
        return Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode &&
                                                       (int)response.StatusCode >= 500)
            .WaitAndRetryAsync(
                retryCount: maxRetryCount,
                sleepDurationProvider: retryAttempt => retryDelay * Math.Pow(2, retryAttempt - 1),
                onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
                {
                    var message = outcome.Exception != null
                        ? $"Request failed, will retry in {timespan.TotalSeconds} seconds (attempt {retryAttempt}): {outcome.Exception.Message}"
                        : $"Request returned {outcome.Result.StatusCode}, will retry in {timespan.TotalSeconds} seconds (attempt {retryAttempt})";

                    Log4Logger.Logger.Warn(message);
                    await Task.CompletedTask;
                }
            );
    }
    
    #endregion IServiceCollection
    
    #region IHttpClientBuilder

    extension(IHttpClientBuilder clientBuilder)
    {
        public void AddHttpClientInterceptor<THandler>() where THandler : DelegatingHandler
        {
            if (clientBuilder == null)
            {
                throw new InvalidOperationException("IHttpClientBuilder is not created, please add an HttpClient first.");
            }

            clientBuilder.AddHttpMessageHandler<THandler>();
        }
        
        public IHttpClientBuilder AddPolicyHandler<THandler>(
            Func<IServiceProvider, THandler> buildPolicyHandler)
            where THandler : DelegatingHandler
        {
            if (clientBuilder == null)
            {
                throw new ArgumentNullException(nameof(clientBuilder));
            }

            if (buildPolicyHandler == null)
            {
                throw new ArgumentNullException(nameof(buildPolicyHandler));
            }

            clientBuilder.AddHttpMessageHandler(buildPolicyHandler.Invoke);

            return clientBuilder;
        }
    }
    
    #endregion IHttpClientBuilder
    
    #region IServiceProvider

    extension(IServiceProvider serviceProvider)
    {
        public TClient GetHttpClient<TClient>(string clientIdentifier) where TClient : class, IHttpApiClient
        {
            if (serviceProvider == null)
            {
                throw new InvalidOperationException(
                    "Service provider is not built. Please call BuildServiceProvider() before requesting HttpClient.");
            }

            return serviceProvider.GetRequiredKeyedService<TClient>(clientIdentifier);
        }
        
        public IHttpApiClientOptions GetClientOptions(string clientIdentifier)
        {
            ArgumentNullException.ThrowIfNull(clientIdentifier);

            try
            {
                return serviceProvider
                    .GetRequiredService<IOptionsMonitor<HttpApiClientOptions>>()
                    .Get(clientIdentifier);
            }
            catch (Exception e)
            {
                throw new HttpException(e, $"GetHttpApiClient options failed for identifier[{clientIdentifier}]. Error: {e.Message}.", HttpStatus.Configuration);
            }
        }
    }

    #endregion IServiceProvider
}