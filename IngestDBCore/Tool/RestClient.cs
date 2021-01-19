using IngestDBCore.Dto;
using Sobey.Core.Log;
using Sobey.Ingest.CommonHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IngestDBCore.Tool
{
    public class RestClient : IDisposable
    {
        ILogger Logger = LoggerManager.GetLogger("ApiClient");

        private HttpClient _httpClient = null;
        private bool _disposed;
        public RestClient(IHttpClientFactory httpClientFactory, string httpClientName = "ApiClient")
        {
            _disposed = false;
            _httpClient = httpClientFactory != null ? httpClientFactory.CreateClient(httpClientName) : new HttpClient();
            _httpClient.DefaultRequestHeaders.Connection.Clear();
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("sobeyhive-http-system", "INGESTSERVER");
            _httpClient.DefaultRequestHeaders.Add("sobeyhive-http-site", "S1");
            _httpClient.DefaultRequestHeaders.Add("sobeyhive-http-tool", "INGESTSERVER");
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RestClient()
        {
            //必须为false
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {

            }
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
            _disposed = true;
        }
        public Dictionary<string, string> GetTokenHeader(string usertoken)
        {
            return new Dictionary<string, string>() {
                {"sobeyhive-http-token", usertoken },
                {"sobeyhive-http-request-id", Guid.NewGuid().ToString("N")}
            };
        }

        public Dictionary<string, string> GetCodeHeader(string usertoken)
        {
            return new Dictionary<string, string>() {
                {"sobeyhive-http-secret", RSAHelper.RSAstr()},
                {"current-user-code", usertoken },
                {"sobeyhive-http-request-id", Guid.NewGuid().ToString("N")}
            };
        }
       
        //public void UseCodeHeader(string usercode)
        //{
        //    if (_httpClient.DefaultRequestHeaders.Contains("sobeyhive-http-token"))
        //    {
        //        _httpClient.DefaultRequestHeaders.Remove("sobeyhive-http-token");
        //    }

        //    if (_httpClient.DefaultRequestHeaders.Contains("sobeyhive-http-secret"))
        //    {
        //        _httpClient.DefaultRequestHeaders.Remove("sobeyhive-http-secret");
        //    }
        //    if (_httpClient.DefaultRequestHeaders.Contains("current-user-code"))
        //    {
        //        _httpClient.DefaultRequestHeaders.Remove("current-user-code");
        //    }

        //    _httpClient.DefaultRequestHeaders.Add("sobeyhive-http-secret", RSAHelper.RSAstr());
        //    _httpClient.DefaultRequestHeaders.Add("current-user-code", usercode);
        //}

        //public void UseTokenHeader(string usertoken)
        //{
        //    if (_httpClient.DefaultRequestHeaders.Contains("sobeyhive-http-secret"))
        //    {
        //        _httpClient.DefaultRequestHeaders.Remove("sobeyhive-http-secret");
        //    }
        //    if (_httpClient.DefaultRequestHeaders.Contains("current-user-code"))
        //    {
        //        _httpClient.DefaultRequestHeaders.Remove("current-user-code");
        //    }

        //    if (_httpClient.DefaultRequestHeaders.Contains("sobeyhive-http-token"))
        //    {
        //        _httpClient.DefaultRequestHeaders.Remove("sobeyhive-http-token");
        //    }

        //    _httpClient.DefaultRequestHeaders.Add("sobeyhive-http-token", usertoken);
        //}

        public async Task<TResponse> Post<TResponse>(string url, object body, Dictionary<string, string> header, string method = "POST", NameValueCollection queryString = null, int timeout = 60)
            where TResponse : class, new()
        {
            TResponse response = null;
            try
            {
                string json = JsonHelper.ToJson(body);
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }
                
                url = CreateUrl(url, queryString);
                //Logger.Debug("请求：{0} {1}", method, url);
                byte[] strData = Encoding.UTF8.GetBytes(json);
                MemoryStream ms = new MemoryStream(strData);
                using (StreamContent sc = new StreamContent(ms))
                {
                    sc.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                    if (header != null)
                    {
                        foreach (var item in header)
                        {
                            sc.Headers.Add(item.Key, item.Value);
                        }
                    }

                    var res = await client.PostAsync(url, sc).ConfigureAwait(true);
                    byte[] rData = await res.Content.ReadAsByteArrayAsync().ConfigureAwait(true);
                    string rJson = Encoding.UTF8.GetString(rData);
                    //Logger.Debug("应答：\r\n{0}", rJson);
                    response = JsonHelper.ToObject<TResponse>(rJson);
                    return response;
                }
            }
            catch (System.Exception e)
            {
                TResponse r = new TResponse();
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
                throw;
            }
        }

        public async Task<string> Post(string url, string body, string method = "POST", NameValueCollection queryString = null, int timeout = 60)
        {
            string response = null;
            try
            {
                string json = body;
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }
                if (String.IsNullOrEmpty(method))
                {
                    method = "POST";
                }
                url = CreateUrl(url, queryString);
                //Logger.Debug("请求：{0} {1}", method, url);
                byte[] strData = Encoding.UTF8.GetBytes(json);
                MemoryStream ms = new MemoryStream(strData);
                StreamContent sc = new StreamContent(ms);
                sc.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                var res = await client.PostAsync(url, sc);
                if (res.Content == null || res.Content.Headers.ContentLength == 0)
                {
                    response = "";
                }
                else
                {
                    byte[] rData = await res.Content.ReadAsByteArrayAsync();
                    string rJson = Encoding.UTF8.GetString(rData);
                    //Logger.Debug("应答：\r\n{0}", rJson);
                    response = rJson;
                }
            }
            catch (System.Exception e)
            {
                response = "ERROR";
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
            }
            return response;
        }

        public async Task<TResponse> Get<TResponse>(string url, NameValueCollection queryString, Dictionary<string, string> header)
                    where TResponse : class, new()
        {
            TResponse response = null;
            try
            {
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }
                url = CreateUrl(url, queryString);
                //Logger.Debug("请求：{0} {1}", "GET", url);
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    if (header != null)
                    {
                        foreach (var item in header)
                        {
                            requestMessage.Headers.Add(item.Key, item.Value);
                        }
                    }
                    var backinfo = await client.SendAsync(requestMessage).ConfigureAwait(true);
                    var rJson = await backinfo.Content.ReadAsStringAsync().ConfigureAwait(true);
                    Logger.Info("url response：\r\n{0} {1}", url, rJson);
                    response = JsonHelper.ToObject<TResponse>(rJson);
                }
            }
            catch (System.Exception e)
            {
                TResponse r = new TResponse();
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
                return r;
            }
            return response;
        }

        public async Task<TResponse> GetCmApi<TResponse>(string url, NameValueCollection queryString = null)
        {
            TResponse response = default(TResponse);
            try
            {
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }
                url = CreateUrl(url, queryString);
                client.DefaultRequestHeaders.SetHeaderValue();
                //Logger.Debug("请求：{0} {1}", "GET", url);
                byte[] rData = await client.GetByteArrayAsync(url);
                string rJson = Encoding.UTF8.GetString(rData);
                //Logger.Debug("应答：\r\n{0}", rJson);
                response = JsonHelper.ToObject<TResponse>(rJson);
            }
            catch (System.Exception e)
            {
                TResponse r = default(TResponse);
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
                return r;
            }
            return response;
        }

        

        public async Task<string> Post(string url, object body, string method, NameValueCollection queryString)
        {
            string response = null;
            try
            {
                string json = JsonHelper.ToJson(body);
                HttpClient client = _httpClient;
                if (queryString == null)
                {
                    queryString = new NameValueCollection();
                }

                url = CreateUrl(url, queryString);
                if (String.IsNullOrEmpty(method))
                {
                    method = "POST";
                }
                //Logger.Debug("请求：{0} {1}", method, url);
                byte[] strData = Encoding.UTF8.GetBytes(json);
                MemoryStream ms = new MemoryStream(strData);
                StreamContent sc = new StreamContent(ms);
                sc.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
                var res = await client.PostAsync(url, sc);
                byte[] rData = await res.Content.ReadAsByteArrayAsync();
                string rJson = Encoding.UTF8.GetString(rData);
                //Logger.Debug("应答：\r\n{0}", rJson);
                response = rJson;
                return response;
            }
            catch (System.Exception e)
            {
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
                throw;
            }
        }

        public async Task<TResult> PostWithToken<TResult>(string url, object body, string token, string userId = null, string method = "Post")
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            string apiUrl = $"{url}";
            HttpMethod hm = new HttpMethod(method);

            var request = new HttpRequestMessage(hm, apiUrl);
            if (!String.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            }
            if (!String.IsNullOrEmpty(userId))
            {
                request.Headers.Add("User", userId);
            }

            string json = "";
            if (body != null)
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            }
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException("验证失败");
            }
            try
            {
                response.EnsureSuccessStatusCode();
                string str = await response.Content.ReadAsStringAsync();

                //sw.Stop();
                //if (sw.ElapsedMilliseconds >= 1000)
                //{
                //slowLogger.Warn("请求时间超过一秒：POST {0} {1}", apiUrl, sw.ElapsedMilliseconds);
                //}

                return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(str);
            }
            catch (Exception e)
            {
                //logger.Error("Post 失败：{0}\r\n{1}", url, e.ToString());
                string str = await response.Content.ReadAsStringAsync();
                //logger.Error(str);
                throw;
            }
        }

        public async Task<TResult> SubmitForm<TResult>(string url, Dictionary<string, string> formData, string method = "Post")
        {
            HttpMethod hm = new HttpMethod(method);
            var request = new HttpRequestMessage(hm, url);
            request.Content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new UnauthorizedAccessException("验证失败");
            }
            response.EnsureSuccessStatusCode();
            string str = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TResult>(str);
        }


        public static string CreateUrl(string url, NameValueCollection qs)
        {
            if (qs != null && qs.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                List<string> kl = qs.AllKeys.ToList();
                foreach (string k in kl)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append("&");
                    }
                    sb.Append(k).Append("=");
                    if (!String.IsNullOrEmpty(qs[k]))
                    {

                        sb.Append(System.Net.WebUtility.UrlEncode(qs[k]));
                    }
                }


                if (url.Contains("?"))
                {
                    url = url + "&" + sb.ToString();
                }
                else
                {
                    url = url + "?" + sb.ToString();
                }
            }

            return url;

        }

        #region cmapi接口统一管理，方便后面修改
        public async Task<string> GetGlobalParam(bool usetokencode, string userTokenOrCode, string key)
        {
            Dictionary<string, string> header = null;
            if (usetokencode)
                header = GetTokenHeader(userTokenOrCode);
            else
                header = GetCodeHeader(userTokenOrCode);
            var back = await AutoRetry.Run<ResponseMessage<CmParam>>(() =>
            {
                DefaultParameter param = new DefaultParameter()
                {
                    tool = "DEFAULT",
                    paramname = key,
                    system = "INGEST"
                };
                return Post<ResponseMessage<CmParam>>(
                string.Format("{0}/CMApi/api/basic/config/getsysparam", ApplicationContext.Current.CMServerUrl),
                param, header);
            });

            //if (back != null)
            //{
            //    return back.Ext?.paramvalue;
            //}
            //有polly重试了，手动重试放后面用
            //DefaultParameter param = new DefaultParameter()
            //{
            //    tool = "DEFAULT",
            //    paramname = key,
            //    system = "INGEST"
            //};
            //var back = await Post<ResponseMessage<CmParam>>(
            //string.Format("{0}/CMApi/api/basic/config/getsysparam", ApplicationContext.Current.CMServerUrl),
            //param);
            if (back != null)
            {
                return back.Ext?.paramvalue;
            }
            return string.Empty;
        }


        public async Task<int> GetUserParamTemplateID(bool usetokencode, string userTokenOrCode, string site = "")
        {
            Dictionary<string, string> header = null;
            if (usetokencode)
                header = GetTokenHeader(userTokenOrCode);
            else
                header = GetCodeHeader(userTokenOrCode);

            if (!string.IsNullOrEmpty(site))
            {
                header.Add("sobeyhive-http-site", site);
            }

            var back = await AutoRetry.Run<ResponseMessage<CmParam>>(() =>
                {
                    DefaultParameter param = new DefaultParameter()
                    {
                        tool = "DEFAULT",
                        paramname = "HIGH_RESOLUTION",
                        system = "INGEST"
                    };
                    return Post<ResponseMessage<CmParam>>(
                    string.Format("{0}/CMApi/api/basic/config/getuserparam", ApplicationContext.Current.CMServerUrl),
                    param, header);

                });

            //有polly重试了，手动重试放后面用
            //DefaultParameter param = new DefaultParameter()
            //{
            //    tool = "DEFAULT",
            //    paramname = "HIGH_RESOLUTION",
            //    system = "INGEST"
            //};
            //var back = await Post<ResponseMessage<CmParam>>(
            //string.Format("{0}/CMApi/api/basic/config/getuserparam", ApplicationContext.Current.CMServerUrl),
            //param);

            if (back != null && !string.IsNullOrEmpty(back.Ext?.paramvalue))
            {
                return int.Parse(back.Ext?.paramvalue);
            }
            return 0;
        }

        public async Task<string> GetUserPath(bool usetokencode, string userTokenOrCode, string storagetype, string storagemark)
        {
            Dictionary<string, string> header = null;
            if (usetokencode)
                header = GetTokenHeader(userTokenOrCode);
            else
                header = GetCodeHeader(userTokenOrCode);


            var back = await AutoRetry.Run<ResponseMessage<ExtParam>>(() =>
            {

                NameValueCollection v = new NameValueCollection();
                v.Add("storagetype", storagetype);
                v.Add("storagemark", storagemark);
                return Get<ResponseMessage<ExtParam>>(
                    string.Format("{0}/CMApi/api/basic/user/getcurrentusercanwritepathbycondition", ApplicationContext.Current.CMServerUrl),
                    v, header);

            });
                //有polly重试了，手动重试放后面用
            //    NameValueCollection v = new NameValueCollection();
            //v.Add("storagetype", storagetype);
            //v.Add("storagemark", storagemark);
            //var back = await Get<ResponseMessage<ExtParam>>(
            //    string.Format("{0}/CMApi/api/basic/user/getcurrentusercanwritepathbycondition", ApplicationContext.Current.CMServerUrl),
            //    v);

            if (back != null)
            {
                return back.Ext.path;
            }
            return string.Empty;
        }

        public async Task<CMUserInfo> GetUserInfo(bool usetokencode, string userTokenOrCode, string userCode)
        {
            Dictionary<string, string> header = null;
            if (usetokencode)
                header = GetTokenHeader(userTokenOrCode);
            else
                header = GetCodeHeader(userTokenOrCode);


            var back = await AutoRetry.Run<ResponseMessage<CMUserInfo>>(() =>
            {

                NameValueCollection v = new NameValueCollection();
                v.Add("usercode", userCode);
                return Get<ResponseMessage<CMUserInfo>>(
                    string.Format("{0}/CMApi/api/basic/account/getuserinfobyusercode", ApplicationContext.Current.CMServerUrl),
                    v, header);
            });

            //NameValueCollection v = new NameValueCollection();
            //v.Add("usercode", userCode);
            //var back = await Get<ResponseMessage<CMUserInfo>>(
            //    string.Format("{0}/CMApi/api/basic/account/getuserinfobyusercode", ApplicationContext.Current.CMServerUrl),
            //    v);

            if (back != null)
            {
                return back.Ext;
            }
            return null;
        }


        #endregion
    }
}
