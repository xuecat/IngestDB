using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngestTaskPlugin.Extend
{
    public static class ConvertExtend
    {
        public static DateTime ToDateTime(this string strTime, DateTime defaultValue = default(DateTime))
        {
            try
            {
                if (DateTime.TryParse(strTime, out DateTime value))
                    return value;
                if (strTime == string.Empty || strTime.Length < 19)
                    return defaultValue;
                IFormatProvider culture = new CultureInfo("fr-FR", true);
                DateTime dt = DateTime.ParseExact(strTime, "yyyy-MM-dd HH:mm:ss", culture);
                return dt;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string ToStr(this DateTime date)
        {
            try
            {
                return date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
