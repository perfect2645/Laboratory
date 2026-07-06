# Messaging.Client.perfect2645

## HttpClient

- Integrates with Microsoft.Extensions.Http for dependency injection and configuration. 
- It provides a simple and reliable way to send HTTP requests and receive responses from a resource.
- Supports retry policies, timeout settings, and custom headers.
- Supports configuration through appsettings.json. Below is an example of how to configure the HttpClient in your appsettings.json file:

``` json
{
  "ApiSettings": {
    "Http": [
      {
        "ApiKey" : "shirts",
        "Resource" : "shirts",
        "BaseUrl": "https://localhost:7029/api/",
        "Timeout": 45,
        "RetryPolicy": {
          "MaxRetryCount" : 4,
          "RetryDelay" : 1
        }
      },
      {
        "ApiKey" : "demo",
        "BaseUrl": "https://localhost:7029/api/",
        "Timeout": 10,
        "Policy": "RetryPolicy"
      }
    ]
  }
}

### Demo of how to use the HttpClient in your application:

[Simple usage of the HttpApiClient](https://github.com/perfect2645/Laboratory/blob/main/MessagingTest/Http/HttpApiClientDemo.cs)

[Custom your HttpApiClient](https://github.com/perfect2645/Laboratory/blob/main/MessagingTest/Http/CustomHttpClientDemo.cs)

Recommend: [Framework usage of the HttpApiClient](https://github.com/perfect2645/Laboratory/blob/main/MessagingTest/Http/FrameworkHttpApiClientDemo.cs)


        