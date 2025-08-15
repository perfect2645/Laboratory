namespace Utils.Generic
{
    public static class ObjectExt
    {
        public static int ToInt(this object? obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var intResult = int.TryParse(obj.ToString(), out int result);
            if (!intResult)
            {
                var doubleResult = double.TryParse(obj.ToString(), out double doubleTry);
                if (doubleResult)
                {
                    return Convert.ToInt32(doubleTry);
                }
            }
            return Convert.ToInt32(obj);
        }

        public static double ToDouble(this object? obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var doubleResult = double.TryParse(obj.ToString(), out double result);
            if (doubleResult)
            {
                return result;
            }
            return Convert.ToDouble(obj);
        }

        public static string NotNullString(this object? source)
        {
            var strSource = source?.ToString();
            return strSource ?? string.Empty;
        }
    }
}
