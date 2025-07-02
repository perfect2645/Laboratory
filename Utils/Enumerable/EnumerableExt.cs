namespace Utils.Enumerable
{
    public static class EnumerableExt
    {
        public static bool HasItem<T>(this IEnumerable<T>? list)
        {
            if (list == null)
            {
                return false;
            }

            return list.Count() > 0;
        }
    }
}
