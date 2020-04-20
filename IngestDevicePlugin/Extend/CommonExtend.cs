using System;
using System.Globalization;

namespace IngestDevicePlugin.Extend
{
    public static class CommonExtend
    {
        public static int ToInt32(this string str, int defaultValue = 0)
        {
            if (Int32.TryParse(str, out int value))
                return value;
            return defaultValue;
        }

        public static int ToInt32(this int? str, int defaultValue = 0)
        {
            return str ?? defaultValue;
        }

        public static DateTime ToDateTime(this string strTime)
        {
            try
            {
                if (DateTime.TryParse(strTime, out DateTime value))
                    return value;
                if (strTime == string.Empty || strTime.Length < 19)
                    return DateTime.MinValue;
                IFormatProvider culture = new CultureInfo("fr-FR", true);
                DateTime dt = DateTime.ParseExact(strTime, "yyyy-MM-dd HH:mm:ss", culture);
                return dt;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}
