using Messaging.Http.Configurations;
using Messaging.Http.Exceptions;
using Messaging.Http.Ioc;
using MessagingTest.Http.CustomHttpClientSample;
using MessagingTest.Http.CustomHttpClientSample.Policy;
using Microsoft.Extensions.DependencyInjection;
using Utils.Configuration;
using Utils.Tasking;

namespace MessagingTest.Http;

public class CustomHttpClientDemo
{
    #region Init
    
    private readonly HttpClientContainer _httpClientContainer;
    private const string ClientKey = "customHttpClient";
    
    public CustomHttpClientDemo()
    {
        InitConfigurations();
        
        _httpClientContainer = HttpClientContainer.Instance;
        RegisterClient();
        
        GetCustomClient().SafeFireAndForget();
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

            serviceCollection.AddHttpApiClient(ClientKey, () =>
            {
                var httpSettings = HttpConfigHelper.ReadFromConfig("ApiSettings:Http", "shirts");
                httpSettings ??= new HttpApiClientConfig();
                return httpSettings.ToApiClientOptions();
            })
            .AddPolicyHandler<CustomHttpRequestRetryHandler>(serviceProvider =>
            {
                var options = serviceProvider.GetClientOptions(ClientKey);
                return new CustomHttpRequestRetryHandler(options);
            });
            
            serviceCollection.AddCustomHttpClient(ClientKey, (serviceProvider, httpClient, options)
                => new SampleCustomHttpClient(httpClient));
        });
    }

    #endregion Register HttpClient
    
    #region GetClient
    
    private async Task GetCustomClient()
    {
        try
        {
            var customClient =
                _httpClientContainer.ServiceProvider.GetRequiredKeyedService<SampleCustomHttpClient>(ClientKey);

            var response = await customClient.GetAsync("shirts", null);

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
}