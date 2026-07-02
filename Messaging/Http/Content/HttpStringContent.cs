using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Utils.Enumerable;
using Utils.Generic;
using Utils.Json;

namespace Messaging.Http.Content
{
    public class HttpStringContent(string url) : IHttpApiContent
    {
        #region Properties
        
        private readonly Encoding _utf8 = Encoding.UTF8;

        private readonly Dictionary<string, string> _headers = new();
        private readonly Dictionary<string, string> _contentHeaders = new();
        private readonly Dictionary<string, object> _bodyPairs = new();

        public IReadOnlyDictionary<string, string> Headers => _headers;
        public IReadOnlyDictionary<string, string> ContentHeaders => _contentHeaders;
        public IReadOnlyDictionary<string, object> Content => _bodyPairs;
        public MediaTypeHeaderValue ContentType { get; set; } = MediaTypeHeaderValue.Parse("application/json");
        public string RequestUrl { get; } = url;

        #endregion Properties

        #region Header

        public void AddHeader(string key, string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
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
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
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
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
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

        public virtual StringContent GetJsonContent()
        {
            var json = JsonSerializer.Serialize(_bodyPairs, JsonEncoder.JsonOption);
            return new StringContent(json, _utf8, ContentType);
        }

        public virtual StringContent GetStringContent()
        {
            var keyValueList = _bodyPairs
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value.ToString() ?? string.Empty)}");

            var formData = string.Join("&", keyValueList);
            return new StringContent(formData, _utf8, ContentType);
        }

        public virtual StringContent GetArrayContent()
        {
            var sb = new StringBuilder();
            foreach (var value in Content.Values)
            {
                sb.Append($"{value},");
            }
            var stringContent = sb.ToString().TrimEnd(',');
            stringContent = $"[{stringContent}]";

            return new StringContent(stringContent, _utf8, ContentType);
        }
        
        #endregion Content
    }
}
