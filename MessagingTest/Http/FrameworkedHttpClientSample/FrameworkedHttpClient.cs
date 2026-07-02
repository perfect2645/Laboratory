using Messaging.Http.Client;

namespace MessagingTest.Http.FrameworkedHttpClientSample;

public class FrameworkedHttpClient(HttpClient httpClient) : HttpApiClient(httpClient)
{
    
}