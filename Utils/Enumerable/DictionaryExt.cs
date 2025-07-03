using System.Collections.Concurrent;
using System.Text.Json;
using Utils.Json;

namespace Utils.Enumerable
{
    public static class DictionaryExt
    {
        private static object UpdateLock = new object();

        #region Dictionary

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value) where TKey : notnull
        {
            lock (UpdateLock)
            {
                dic = dic ?? new Dictionary<TKey, TValue>();
                if (dic.ContainsKey(key))
                {
                    dic[key] = value;
                }
                else
                {
                    dic.Add(key, value);
                }
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd) where TKey : notnull
        {
            dic = dic ?? new Dictionary<TKey, TValue>();

            if (dicToAdd == null)
            {
                return;
            }

            foreach (var pair in dicToAdd)
            {
                dic.AddOrUpdate(pair.Key, pair.Value);
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> source, TKey key) where TKey : notnull
        {
            lock (UpdateLock)
            {
                dic = dic ?? new Dictionary<TKey, TValue>();
                if (dic.ContainsKey(key))
                {
                    dic[key] = source[key];
                }
                else
                {
                    dic.Add(key, source[key]);
                }
            }
        }


        public static string ToJson(this Dictionary<string, object> dic)
        {
            var json = JsonSerializer.Serialize(dic, JsonEncoder.JsonOption);

            return json;
        }

        public static string ToJson(this Dictionary<string, string> dic)
        {
            var json = JsonSerializer.Serialize(dic, JsonEncoder.JsonOption);

            return json;
        }

        public static bool HasItem(this Dictionary<string, object> dic)
        {
            if (dic == null)
            {
                return false;
            }

            return dic.Count > 0;
        }

        public static string? GetString(this Dictionary<string, object> dic, string key)
        {
            if (dic == null || key == null)
            {
                return null;
            }

            if (!dic.ContainsKey(key))
            {
                return null;
            }

            if (dic[key] == null)
            {
                return null;
            }
            return dic[key].ToString();
        }

        public static int GetInt(this Dictionary<string, object> dic, string key)
        {
            var strValue = dic.GetString(key);
            return strValue.ToInt();
        }

        public static object? GetValue(this Dictionary<string, object> dic, string key)
        {
            if (dic == null || key == null)
            {
                return null;
            }

            if (!dic.ContainsKey(key))
            {
                return null;
            }
            return dic[key];
        }

        #endregion Dictionary

        #region ConcurrentDictionary

        public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dic, TKey key, TValue value) where TKey : notnull
        {
            lock (UpdateLock)
            {
                dic = dic ?? new ConcurrentDictionary<TKey, TValue>();
                if (dic.ContainsKey(key))
                {
                    dic[key] = value;
                }
                else
                {
                    dic.TryAdd(key, value);
                }
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dic, ConcurrentDictionary<TKey, TValue> dicToAdd) where TKey : notnull
        {
            dic = dic ?? new ConcurrentDictionary<TKey, TValue>();

            if (dicToAdd == null)
            {
                return;
            }

            foreach (var pair in dicToAdd)
            {
                dic.AddOrUpdate(pair.Key, pair.Value);
            }
        }

        public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dic, ConcurrentDictionary<TKey, TValue> source, TKey key) where TKey : notnull
        {
            lock (UpdateLock)
            {
                dic = dic ?? new ConcurrentDictionary<TKey, TValue>();
                if (dic.ContainsKey(key))
                {
                    dic[key] = source[key];
                }
                else
                {
                    dic.TryAdd(key, source[key]);
                }
            }
        }


        public static string ToJson(this ConcurrentDictionary<string, object> dic)
        {
            var json = JsonSerializer.Serialize(dic, JsonEncoder.JsonOption);

            return json;
        }

        public static bool HasItem(this ConcurrentDictionary<string, object> dic)
        {
            if (dic == null)
            {
                return false;
            }

            return dic.Count > 0;
        }

        public static string? GetString(this ConcurrentDictionary<string, object> dic, string key)
        {
            if (dic == null || key == null)
            {
                return null;
            }

            if (!dic.ContainsKey(key))
            {
                return null;
            }
            return dic[key].ToString();
        }

        public static object? GetValue(this ConcurrentDictionary<string, object> dic, string key)
        {
            if (dic == null || key == null)
            {
                return null;
            }

            if (!dic.ContainsKey(key))
            {
                return null;
            }
            return dic[key];
        }

        #endregion ConcurrentDictionary
    }
}
