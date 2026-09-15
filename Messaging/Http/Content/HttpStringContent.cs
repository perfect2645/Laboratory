using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Utils.Enumerable;
using Utils.Generic;
using Utils.Json;

namespace Messaging.Http.Content
{
    public class HttpStringContent : IHttpApiContent
    {
        #region Properties
        
        private readonly Encoding _utf8 = Encoding.UTF8;

        private readonly Dictionary<string, string> _headers = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _contentHeaders = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, object> _bodyPairs = new(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, string> Headers => _headers;
        public IReadOnlyDictionary<string, string> ContentHeaders => _contentHeaders;
        public IReadOnlyDictionary<string, object> Content => _bodyPairs;
        public MediaTypeHeaderValue ContentType { get; set; } = MediaTypeHeaderValue.Parse("application/json");
        public string? RelativeUri { get; init; }

        #endregion Properties

        #region Header

        public void AddHeader(string key, string value)
        {
            _headers.AddOrUpdate(key, value);
        }

        public void AddHeader(Dictionary<string, object> source, string key)
        {
            if (source.TryGetValue(key, out var val))
                AddHeader(key, val.NotNullString());
        }

        public void AddHeaders(Dictionary<string, string> pairs)
        {
            if (pairs.HasItem())
                _headers.AddOrUpdate(pairs);
        }

        #endregion Header
        
        #region Content Header

        public void AddContentHeader(string key, string value)
        {
            _contentHeaders.AddOrUpdate(key, value);
        }

        public void AddContentHeader(Dictionary<string, object> source, string key)
        {
            if (source.TryGetValue(key, out var val))
                AddContentHeader(key, val.NotNullString());
        }

        public void AddContentHeaders(Dictionary<string, string> pairs)
        {
            if (pairs.HasItem())
                _contentHeaders.AddOrUpdate(pairs);
        }

        #endregion Content Header

        #region Content

        public void AddContent(string key, object value)
        {
            _bodyPairs.AddOrUpdate(key, value);
        }

        public void AddContent(Dictionary<string, object> source, string key)
        {
            if (source.TryGetValue(key, out var val))
                AddContent(key, val);
        }

        public void AddContents(Dictionary<string, object> pairs)
        {
            if (pairs.HasItem())
                _bodyPairs.AddOrUpdate(pairs);
        }

        public virtual JsonContent GetJsonContent()
        {
            return JsonContent.Create(_bodyPairs, ContentType, JsonEncoder.JsonOption);
        }

        public virtual HttpContent GetFormContent()
        {
            var keyValueList = _bodyPairs
                .Select(pair => new KeyValuePair<string, string>(
                    pair.Key, 
                    Convert.ToString(pair.Value, CultureInfo.InvariantCulture) ?? string.Empty));
            return new FormUrlEncodedContent(keyValueList);
        }
        
        #endregion Content
    }
}
