using System.Net.Http.Headers;

namespace Messaging.Http.Content
{
    public interface IHttpApiContent
    {
        IReadOnlyDictionary<string, string> Headers { get; }
        IReadOnlyDictionary<string, string> ContentHeaders { get; }
        IReadOnlyDictionary<string, object> Content { get; }
        MediaTypeHeaderValue ContentType { get; set; }
        string RequestUrl { get; }
        StringContent GetJsonContent();
        StringContent GetStringContent();
    }
}
