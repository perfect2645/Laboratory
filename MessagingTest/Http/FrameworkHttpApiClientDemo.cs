using Messaging.Http.Client;
using Messaging.Http.Configurations;
using Messaging.Http.Exceptions;
using Messaging.Http.Ioc;
using MessagingTest.Http.FrameworkedHttpClientSample;
using Utils.Configuration;
using Utils.Tasking;

namespace MessagingTest.Http;

public class FrameworkHttpApiClientDemo
{
    #region Init

    private const string ApiKey = "shirts";
    private readonly HttpClientContainer _httpClientContainer;
    public FrameworkHttpApiClientDemo()
    {
        InitConfigurations();
        
        _httpClientContainer = HttpClientContainer.Instance;
        RegisterClient();

        TestHttpGetRequests().SafeFireAndForget();

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
            serviceCollection.AddConfiguratedHttpClient<FrameworkedHttpClient>(ApiKey);
        });

    }
    
    #endregion Register HttpClient
    
    #region GetClient

    private async Task TestHttpGetRequests()
    {
        try
        {
            var client = _httpClientContainer.ServiceProvider.GetHttpClient<FrameworkedHttpClient>(ApiKey);
            var shirts = await client.GetStringAsync();
            
            Console.WriteLine(shirts);
        }
        catch (HttpException ex)
        {
            Console.WriteLine(ex.GetDetailedMessages());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    
    #endregion GetClient
}