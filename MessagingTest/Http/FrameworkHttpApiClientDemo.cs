using Messaging.Http.Client;

namespace MessagingTest.Http;

public class FrameworkHttpApiClientDemo(HttpClient httpClient) : HttpApiClient(httpClient)
{
    
}