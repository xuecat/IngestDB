using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Sobey.Ingest.CommonHelper
{
   public static class HeaderHelper
    {
        public static void SetHeaderValue(this HttpContentHeaders Headers)
        {
          Headers.Add("sobeyhive-http-system", "INGEST");
          Headers.Add("sobeyhive-http-site", "S1");
          Headers.Add("sobeyhive-http-tool", "INGESTSERVER");
          Headers.Add("sobeyhive-http-secret", RSAHelper.RSAstr());
          Headers.Add("current-user-code","admin");
        }
        public static void SetHeaderValue(this HttpRequestHeaders Headers)
        {
            Headers.Add("sobeyhive-http-system", "INGEST");
            Headers.Add("sobeyhive-http-site", "S1");
            Headers.Add("sobeyhive-http-tool", "INGESTSERVER");
            Headers.Add("sobeyhive-http-secret", RSAHelper.RSAstr());
            Headers.Add("current-user-code", "admin");
        }
        public static void SetHeaderValue(this HttpRequestHeaders Headers, string code)
        {
            Headers.Add("sobeyhive-http-system", "INGEST");
            Headers.Add("sobeyhive-http-site", "S1");
            Headers.Add("sobeyhive-http-tool", "INGESTSERVER");
            Headers.Add("sobeyhive-http-secret", RSAHelper.RSAstr());
            Headers.Add("current-user-code", code);
        }
    }
}
