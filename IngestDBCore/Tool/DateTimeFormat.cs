﻿using System;
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
                if (strTime == string.Empty || strTime.Length < 19)
                {

                    return DateTime.MinValue;
                }
                IFormatProvider culture = new CultureInfo("fr-FR", true);
                DateTime dt = DateTime.ParseExact(strTime, "yyyy-MM-dd HH:mm:ss", culture);

                return dt;
            }
            catch (System.Exception ex)
            {
                return DateTime.MinValue;
            }
        }
    }
}
