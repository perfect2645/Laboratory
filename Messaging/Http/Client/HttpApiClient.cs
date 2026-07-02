using Logging;
using Messaging.Http.Content;
using Messaging.Http.Exceptions;
using System.Net.Http.Json;
using System.Text.Json;
using Utils.Enumerable;
using Utils.Json;

namespace Messaging.Http.Client
{
    public class HttpApiClient(HttpClient httpClient) : IHttpApiClient
    {
        private const string MediaTypeJson = "application/json";
        private const string MediaTypeForm = "application/x-www-form-urlencoded";
        private const string MediaTypeText = "text/plain";
        
        
        #region Header

        private void AddHeaders(HttpRequestMessage request, IHttpApiContent content)
        {
            if (!content.Headers.HasItem())
                return;

            try
            {
                foreach (var (key, value) in content.Headers.ToList())
                {
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        Log4Logger.Logger.Warn($"Header key is empty, skip this header, value: {value}");
                        continue;
                    }

                    bool addSuccess = request.Headers.TryAddWithoutValidation(key, value);
                    if (!addSuccess)
                    {
                        Log4Logger.Logger.Debug($"Header [{key}] already exists, skip, value: {value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error("HttpClient AddHeaders Error", ex);
                throw new HttpException(ex, ex.Message, HttpStatus.AddRequestHeader);
            }
        }

        #endregion Header
        
        #region Content Header

        private void AddContentHeaders(HttpRequestMessage request, IHttpApiContent content)
        {
            if (!content.ContentHeaders.HasItem())
                return;

            if (request.Content == null)
            {
                throw new HttpException("Must build content before attaching content headers",
                    HttpStatus.AddRequestHeader);
            }

            try
            {
                foreach (var (key, value) in content.ContentHeaders.ToList())
                {
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        Log4Logger.Logger.Warn($"ContentHeader key is empty, skip this header, value: {value}");
                        continue;
                    }

                    bool addSuccess = request.Content.Headers.TryAddWithoutValidation(key, value);
                    if (!addSuccess)
                    {
                        Log4Logger.Logger.Debug($"ContentHeader [{key}] already exists, skip add, value: {value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error("HttpClient Add request content headers failed", ex);
                throw new HttpException(ex, ex.Message, HttpStatus.AddRequestHeader);
            }
        }
        
        #endregion Content Header

        #region Content

        private HttpContent BuildHttpContent(IHttpApiContent content)
        {
            if (content == null)
            {
                throw new HttpException("Http request Content is null.", HttpStatus.NoContent);
            }

            var contentType = content.ContentType.MediaType;
            if (contentType == null)
            {
                throw new HttpException("Http request ContentType is null.", HttpStatus.RequestInvalidContentType);
            }

            try
            {
                return contentType switch
                {
                    var type when type.Equals(MediaTypeJson, StringComparison.OrdinalIgnoreCase)
                        => content.GetJsonContent(),
                    var type when type.Equals(MediaTypeForm, StringComparison.OrdinalIgnoreCase)
                        => content.GetStringContent(),
                    var type when type.Equals(MediaTypeText, StringComparison.OrdinalIgnoreCase)
                        => content.GetStringContent(),
                    _ => JsonContent.Create(content.Content, content.ContentType, JsonEncoder.JsonOption)
                };
            }
            catch (Exception ex)
            {
                Log4Logger.Logger.Error("HttpClient BuildHttpContent Error", ex);
                throw new HttpException(ex, ex.Message, HttpStatus.BuildRequestContent);
            }
        }

        #endregion Content

        #region Methods
        
        public async Task<string?> GetStringAsync(IHttpApiContent? content = null,
            CancellationToken cancellationToken = default)
        {
            content ??= new HttpStringContent(httpClient.BaseAddress!.AbsoluteUri);
            return await SendStringAsync(HttpMethod.Get, content, cancellationToken);
        }

        public async Task<TResponse?> GetAsync<TResponse>(IHttpApiContent? content = null,
            CancellationToken cancellationToken = default) where TResponse : class
        {
            content ??= new HttpStringContent(httpClient.BaseAddress!.AbsoluteUri);

            return await SendRequestAsync<TResponse>(HttpMethod.Get, content, cancellationToken);
        }
        
        public async Task<string?> PostAsync(IHttpApiContent content, 
            CancellationToken cancellationToken = default)
        {
            return await SendStringAsync(HttpMethod.Post, content, cancellationToken);
        }
        
        public async Task<TResponse?> PostAsync<TResponse>(IHttpApiContent content, 
            CancellationToken cancellationToken = default) where TResponse : class
        {
            return await SendRequestAsync<TResponse>(HttpMethod.Post, content, cancellationToken);
        }
        
        private async Task<string?> SendStringAsync(
            HttpMethod method,
            IHttpApiContent  content,
            CancellationToken cancellationToken)
        {
            using var response = await SendHttpRequestInternal(method, content, cancellationToken);
            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            return result;
        }

        private async Task<TResult?> SendRequestAsync<TResult>(
            HttpMethod method,
            IHttpApiContent  content,
            CancellationToken cancellationToken)
            where TResult : class
        {
            try
            {
                using var response = await SendHttpRequestInternal(method, content, cancellationToken);
                byte[] rawResponse = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                if (rawResponse.Length == 0)
                    return null;

                await using var memoryStream = new MemoryStream(rawResponse);
                var result =
                    await JsonSerializer.DeserializeAsync<TResult>(memoryStream, JsonEncoder.JsonOption, cancellationToken);
                return result ?? throw new HttpException("Response deserialize failed", HttpStatus.ResponseDeserialize);
            }
            catch (JsonException ex)
            {
                throw new HttpException(ex, "Failed to Deserialize http response.", HttpStatus.ResponseDeserialize);
            }
        }
        
        private async Task<HttpResponseMessage> SendHttpRequestInternal(HttpMethod httpMethod, 
            IHttpApiContent content, 
            CancellationToken cancellationToken)
        {
            if (content == null)
            {
                throw new HttpException("Http request Content is null.", HttpStatus.NoContent);
            }
            
            if (string.IsNullOrEmpty(content.RequestUrl))
            {
                throw new HttpException("Http request url is empty.", HttpStatus.BadRequestUrl);
            }

            try
            {
                using var request = new HttpRequestMessage(httpMethod, content.RequestUrl);
                AddHeaders(request, content);
                
                if (httpMethod != HttpMethod.Get)
                {
                    request.Content = BuildHttpContent(content);
                    AddContentHeaders(request, content);
                }

                var response = await httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errText = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new HttpException(
                        $"Failed to send http data: {response.ReasonPhrase}:[{response.StatusCode}] {errText}",
                        response.StatusCode.ToHttpStatus());
                }

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new HttpException(ex);
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new HttpException(ex, "Http Request Canceled.", HttpStatus.RequestCancelled);
                }

                throw new HttpException(ex, "Http Request Timeout.", HttpStatus.RequestTimeout);
            }
        }

        #endregion Methods

        #region Log

        public void Log(string message)
        {
            var threadId = Environment.CurrentManagedThreadId;
            Log4Logger.Logger.Info($"[Thread: {threadId}]{message}");
        }

        #endregion Log
    }
}
