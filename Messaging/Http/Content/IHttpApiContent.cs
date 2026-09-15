using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Messaging.Http.Content
{
    public interface IHttpApiContent
    {
        string? RelativeUri { get; }
        IReadOnlyDictionary<string, string> Headers { get; }
        IReadOnlyDictionary<string, string> ContentHeaders { get; }
        IReadOnlyDictionary<string, object> Content { get; }
        MediaTypeHeaderValue ContentType { get; set; }
        JsonContent GetJsonContent();
        HttpContent GetFormContent();
    }
}
