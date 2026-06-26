namespace Messaging.Http.Client;

public static class HttpApiClientOptionsExtensions
{
    extension(HttpApiClientOptions options)
    {
        public void CopyAllFrom(HttpApiClientOptions source)
        {
            ArgumentNullException.ThrowIfNull(source);
        
            options.ApiKey = source.ApiKey;
            options.Resource = source.Resource;
            options.BaseAddress = source.BaseAddress;
            options.Timeout = source.Timeout;
            options.Policy = source.Policy;

            options.DefaultHeaders = new Dictionary<string, string>(source.DefaultHeaders);
        }
    }
}