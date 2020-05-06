using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IngestDBCore.Tool
{
    public class DateTimeFormat
    {
        public static DateTime DateTimeFromString(string strTime)
        {
            try
            {
                if (strTime == string.Empty || strTime.Length < 19)
                {

                    return DateTime.MinValue;
                }
                return DateTime.Parse(strTime);
            }
            catch (System.Exception ex)
            {
                return DateTimeParse2(strTime);
            }
        }
        public static DateTime DateTimeParse2(string strTime)
        {
            try
            {
                
                IFormatProvider culture = new CultureInfo("fr-FR", true);
                DateTime dt = DateTime.ParseExact(strTime, "yyyy-MM-dd HH:mm:ss", culture);

                return dt;
            }
            catch (System.Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        public static string DateTimeToString(DateTime tmDateTime)
        {
            try
            {
                return tmDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (System.Exception ex)
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
