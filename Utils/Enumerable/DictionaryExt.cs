using System.Collections.Concurrent;
using System.Text.Json;
using Utils.Generic;
using Utils.Json;

namespace Utils.Enumerable
{
    public static class DictionaryExt
    {
        #region Dictionary

        extension<TKey, TValue>(Dictionary<TKey, TValue> dic) where TKey : notnull
        {
            public void AddOrUpdate(TKey key, TValue value)
            {
                ArgumentNullException.ThrowIfNull(dic, nameof(dic));
                ArgumentNullException.ThrowIfNull(key, nameof(key));

                if (dic.TryGetValue(key, out _))
                {
                    dic[key] = value;
                    return;
                }
                dic.TryAdd(key, value);
            }

            public void AddOrUpdate(Dictionary<TKey, TValue> dicToAdd)
            {
                ArgumentNullException.ThrowIfNull(dic, nameof(dic));

                if (dicToAdd is null || dicToAdd.Count == 0)
                {
                    return;
                }

                foreach ((TKey key, TValue value) in dicToAdd)
                {
                    dicToAdd.AddOrUpdate(key, value);
                }
            }

            public bool HasItem()
            {
                if (dic is null)
                {
                    return false;
                }

                return dic.Count > 0;
            }
        }


        extension(Dictionary<string, object> dic)
        {
            public string ToJson()
            {
                var json = JsonSerializer.Serialize(dic, JsonEncoder.JsonOption);
                return json;
            }

            public string? GetString(string key)
            {
                if (dic is null || key is null)
                {
                    return null;
                }

                if (dic.TryGetValue(key, out var value))
                {
                    return value.ToString();
                }

                return null;
            }

            public int GetInt(string key)
            {
                var strValue = dic.GetString(key);
                return strValue.ToInt();
            }

            public object? GetValue(string key)
            {
                if (dic is null || key is null)
                {
                    return null;
                }

                if (!dic.ContainsKey(key))
                {
                    return null;
                }
                return dic[key];
            }
        }

        #endregion Dictionary

        #region ConcurrentDictionary

        extension<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dic) where TKey : notnull
        {
            public void AddOrUpdate(TKey key, TValue value)
            {
                ArgumentNullException.ThrowIfNull(dic, nameof(dic));
                ArgumentNullException.ThrowIfNull(key, nameof(key));
                dic.AddOrUpdate(key, value, (_, _) => value);
            }
        }

        extension(ConcurrentDictionary<string, object> dic)
        {
            public string ToJson()
            {
                var json = JsonSerializer.Serialize(JsonEncoder.JsonOption);
                return json;
            }

            public bool HasItem()
            {
                if (dic is null)
                {
                    return false;
                }

                return dic.Count > 0;
            }

            public string? GetString(string key)
            {
                if (dic is null || key is null)
                {
                    return null;
                }

                if (dic.TryGetValue(key, out var value))
                {
                    return value.ToString();
                }

                return null;
            }

            public object? GetValue(string key)
            {
                if (dic is null || key is null)
                {
                    return null;
                }

                if (!dic.ContainsKey(key))
                {
                    return null;
                }
                return dic[key];
            }
        }

        #endregion ConcurrentDictionary
    }
}
