
using Messaging.Http.Content;

namespace Messaging.Http.Client
{
    public interface IHttpApiClient
    {
        Task<string?> GetStringAsync(IHttpApiContent content,
            CancellationToken cancellationToken = default);
        
        Task<TResponse?> GetAsync<TResponse>(IHttpApiContent? content,
            CancellationToken cancellationToken = default) where TResponse : class;

        Task<string?> PostAsync(IHttpApiContent content,
            CancellationToken cancellationToken = default);
        
        Task<TResponse?> PostAsync<TResponse>(IHttpApiContent content,
            CancellationToken cancellationToken = default) where TResponse : class;
    }
}