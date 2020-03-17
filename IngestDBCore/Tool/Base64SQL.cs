using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IngestDBCore.Tool
{
    public class Base64SQL
    {
        private static string ind0 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        private static string ind = "0S1o2b3e4ya7cd8fghijklmn6pqrstuvwx9zABCDEFGHIJKLMNOPQR5TUVWXYZ+/=";
        public Base64SQL()
        {

        }

        //解码
        public static string Base64_Decode(string pSrc)
        {
            string dest = "";
            try
            {
                string src2 = "";
                char[] map = ind.ToArray();
                foreach (char a in pSrc)
                {
                    src2 += map[ind0.IndexOf(a)];
                }

                byte[] bpath = Convert.FromBase64String(src2);
                dest = System.Text.ASCIIEncoding.Default.GetString(bpath);
            }
            catch (Exception)
            {

            }
            return dest;
        }
    }
}
