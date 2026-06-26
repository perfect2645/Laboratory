using System.Net;
using Logging;
using Messaging.Http.Exceptions;

namespace MessagingTest.Http.CustomHttpClientSample;

public class SampleCustomHttpClient(HttpClient httpClient) : ISampleHttpApiClient
{
    private readonly HttpClient _httpClient = httpClient;
    
    public async Task<HttpResponseMessage> GetAsync(
        string? resource,
        string? requestParameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Uri requestUri = BuildRequestUri(resource, requestParameters);

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            string traceId = Guid.NewGuid().ToString("N");
            request.Headers.TryAddWithoutValidation(SampleConstants.TraceIdHeader, traceId);
            request.Headers.TryAddWithoutValidation(SampleConstants.ResourceHeader, resource);
            request.Headers.TryAddWithoutValidation(SampleConstants.RequestParametersHeader, requestParameters);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                HttpStatusCode statusCode = response.StatusCode;
                int numericCode = (int)statusCode;

                throw new HttpRequestException(
                    $"Response status code does not indicate success: {numericCode} ({statusCode})");
            }

            return response;
        }
        catch (HttpRequestException ex)
        {
            Log4Logger.Logger.Error($"HttpRequestException occurred. resource: {resource}, Parameters{requestParameters}. Exception: {ex.Message}");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            Log4Logger.Logger.Error($"TaskCanceledException occurred, http request timeout/cancelled. resource: {resource}, Parameters{requestParameters}. Exception: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Log4Logger.Logger.Error($"Error in GetAsync, resource: {resource}, Parameters{requestParameters}. Exception: {ex.Message}");
            throw;
        }
    }

private Uri BuildRequestUri(string? resource, string? requestParameters)
{
    if (_httpClient.BaseAddress == null)
    {
        throw new HttpException("HttpClient.BaseAddress is null, please set BaseAddress in your configurations");
    }
    
    Uri requestUri = new Uri(_httpClient.BaseAddress, resource);

    if (!string.IsNullOrWhiteSpace(requestParameters))
    {
        var uriBuilder = new UriBuilder(requestUri)
        {
            Query = requestParameters.TrimStart('?')
        };
        requestUri = uriBuilder.Uri;
    }

    return requestUri;
}
}