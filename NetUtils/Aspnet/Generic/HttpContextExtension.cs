using Microsoft.AspNetCore.Http;

namespace NetUtils.Aspnet.Generic
{
    public static class HttpContextExtension
    {
        extension(HttpContext httpContext)
        {
            public bool TryGetValue<T>(string key, out T? value) where T : class
            {
                if (string.IsNullOrEmpty(key))
                {
                    value = null;
                    return false;
                }

                if (httpContext?.Items == null)
                {
                    value = null;
                    return false;
                }

                var target = httpContext!.Items.TryGetValue(key, out var valueFromItems) ? valueFromItems : null;
                value = target as T;
                return value != null;
            }

            public string? GetString(string key)
            {
                TryGetValue(httpContext, key, out string? value);
                return value;
            }
        }
    }
}
