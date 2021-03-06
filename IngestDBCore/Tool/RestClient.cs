﻿using Sobey.Ingest.CommonHelper;
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
    public class RestClient
    {
        //ILogger Logger = LoggerManager.GetLogger("ApiClient");

        private static HttpClient _httpClient = null;

        public RestClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Connection.Clear();
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<TResponse> Post<TResponse>(string url, object body, string method = "POST", NameValueCollection queryString = null, int timeout = 60)
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
                byte[] rData = await res.Content.ReadAsByteArrayAsync();
                string rJson = Encoding.UTF8.GetString(rData);
                //Logger.Debug("应答：\r\n{0}", rJson);
                response = JsonHelper.ToObject<TResponse>(rJson);
                return response;
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

        public async Task<TResponse> Get<TResponse>(string url, NameValueCollection queryString)
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
                byte[] rData = await client.GetByteArrayAsync(url);
                string rJson = Encoding.UTF8.GetString(rData);
                //Logger.Debug("应答：\r\n{0}", rJson);
                response = JsonHelper.ToObject<TResponse>(rJson);
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

        public async Task<TResponse> Post<TResponse>(string url, object body, Dictionary<string, string> header, string method = null, NameValueCollection queryString = null)
        {
            TResponse response = default(TResponse);
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
                foreach (var item in header)
                {
                    sc.Headers.Add(item.Key, item.Value);
                }
                var res = await client.PostAsync(url, sc);
                byte[] rData = await res.Content.ReadAsByteArrayAsync();
                string rJson = Encoding.UTF8.GetString(rData);
                //Logger.Debug("应答：\r\n{0}", rJson);
                response = JsonHelper.ToObject<TResponse>(rJson);
                return response;
            }
            catch (System.Exception e)
            {
                //Logger.Error("请求异常：\r\n{0}", e.ToString());
                throw;
            }
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


    }
}
