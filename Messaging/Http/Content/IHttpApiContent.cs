using System.Net.Http.Headers;

namespace Messaging.Http.Content
{
    public interface IHttpApiContent
    {
        string? RelativeUri { get; }
        IReadOnlyDictionary<string, string> Headers { get; }
        IReadOnlyDictionary<string, string> ContentHeaders { get; }
        IReadOnlyDictionary<string, object> Content { get; }
        MediaTypeHeaderValue ContentType { get; set; }
        StringContent GetJsonContent();
        StringContent GetStringContent();
    }
}
