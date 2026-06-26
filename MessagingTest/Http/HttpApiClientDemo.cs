using Messaging.Http.Client;
using Messaging.Http.Configurations;
using Messaging.Http.Exceptions;
using Messaging.Http.Ioc;
using Messaging.Http.Policies;
using Microsoft.Extensions.Configuration;
using Utils.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utils.Tasking;

namespace MessagingTest.Http;

public class HttpApiClientDemo
{

    #region Init
    
    private readonly HttpClientContainer _httpClientContainer;
    public HttpApiClientDemo()
    {
        InitConfigurations();
        
        _httpClientContainer = HttpClientContainer.Instance;
        RegisterClient();

        TestHttpRequests().SafeFireAndForget();

    }
    
    private void InitConfigurations()
    {
        AppConfig.Init();
    }
    
    #endregion Init
    
    #region Register HttpClient

    private void RegisterClient()
    {
        _httpClientContainer.RegisterService(serviceCollection =>
        {
            // manual add httpclient options
            serviceCollection.AddHttpApiClient("demoHttpClient", clientOptions =>
            {
                clientOptions.BaseAddress = new Uri("https://localhost:5001");
            });
            
            // manual add httpclient options
            serviceCollection.AddHttpClientWithRetryPolicy("demoHttpClientWithRetryPolicy", clientOptions =>
            {
                clientOptions.BaseAddress = new Uri("https://localhost:5001");
                clientOptions.Timeout = TimeSpan.FromSeconds(60);
                clientOptions.Policy = new RetryPolicy
                {
                    EnableRetry = true,
                    MaxRetryCount = 5,
                    RetryDelay = 1,
                };
            });
            
            // hot read httpclient options from appsettings.json
            serviceCollection.AddHttpClientWithRetryPolicy("demoHttpClientFromConfig", () =>
            {
                var httpSettings = HttpConfigHelper.ReadFromConfig("ApiSettings:Http", "shirts");
                httpSettings = httpSettings ?? new HttpApiClientConfig();
                return httpSettings.ToApiClientOptions();
            });
        });
    }

    #endregion Register HttpClient
    
    #region GetClient
    
    private async Task GetNativeHttpClient()
    {
        try
        {
            var httpClientFactory = _httpClientContainer.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("demoHttpClient");

            var response = await client.GetStringAsync("api/test");
            
            // handle your response
        }
        catch (HttpException e)
        {
            Console.WriteLine(e);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
        }
    }
    
    private async Task GetNativeHttpClientWithRetry()
    {
        try
        {
            var httpClientFactory = _httpClientContainer.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("demoHttpClientWithRetryPolicy");

            var response = await client.GetStringAsync("api/test");

            // handle your response
        }
        catch (HttpException e)
        {
            Console.WriteLine(e);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    private async Task GetNativeHttpClientFromConfig()
    {
        try
        {
            var httpClientFactory = _httpClientContainer.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient("demoHttpClientFromConfig");

            var config = _httpClientContainer.ServiceProvider.GetClientOptions("demoHttpClientFromConfig");

            var response = await client.GetStringAsync(config.Resource);

            // handle your response
        }
        catch (HttpException e)
        {
            Console.WriteLine(e);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    #endregion GetClient
    
    #region Test Code

    private async Task TestHttpRequests()
    {
        // await GetNativeHttpClient()

         // await GetNativeHttpClientWithRetry();

         await GetNativeHttpClientFromConfig();
    }
    
    #endregion Test Code
}