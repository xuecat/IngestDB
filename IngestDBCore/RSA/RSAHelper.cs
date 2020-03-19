using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace Sobey.Ingest.CommonHelper
{
   public static class RSAHelper
    {
        public static string RSAstr()
        {
            string publicKey =
                 "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0upEOp0qc6SnEs3uudeyd5BEu98wY7qmh4d3VKfHggSuVzkA1ZFQ5YmUROnCS0k7nJVI7N8XEJw/C2JYDevbizWSYsjiVYSMpxbUs3Ozu/gMaHb1pf32GLKb8lupDQMR20/vILSyIN+3fPN5Ow+j/DNHlqgxAweHJRk1llMhSJY5tkv3PRd0S1w/OyjCB4ZMNuGURkt0QAt8VEauhuE0scr5Ujg95QmGnNE63kPDXHt30b6+YfCczNudHlfRnkjlX4kI7EoAreMRj/GXuooGX+Dg1LPwW1qTwenj1rYBOk6qsCkkCnVE2OTeNYWA4UhpKNwiRUlaFg881rt7oABdIwIDAQAB";
            //RSAHelper rsa = new CommonHelperRSA.RSAHelper(RSAType.RSA2, Encoding.UTF8, null, publicKey);
            var rsa = CommonHelper.ConvertFromPemPublicKey(publicKey);
            RSACryptoServiceProvider rsab = new RSACryptoServiceProvider();
            rsab.ImportParameters(rsa);
            string str = $"INGEST_INGESTSERVER_{ ConvertDateTimeInt(DateTime.Now)}";
            byte[] sample = rsab.Encrypt(Encoding.UTF8.GetBytes(str), false);
            str = Convert.ToBase64String(sample);
            return str; 

        }

        public static long ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }
    }
}
