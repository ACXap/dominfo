using AngleSharp;
using dominfo.Helpers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace dominfo
{
    public class DomInfoRepository
    {
        public DomInfoRepository(string userAgent)
        {
            _userAgent = userAgent;
        }

        public const string URL_SITE = "https://dominfo.ru";

        #region PrivateField
        private const string URL_INIT = "https://dominfo.ru/ajax/protection/init";
        private const string CONTENT_TYPE = "application/x-www-form-urlencoded; charset=UTF-8";

        private readonly CookieContainer _cc = new CookieContainer();
        private readonly string _userAgent;
        private TokenResponse _tokenResponse;
        #endregion PrivateField

        public string LoadPage(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(url));
            request.ContentType = CONTENT_TYPE;
            request.UserAgent = _userAgent;
            request.CookieContainer = _cc;
            request.Timeout = 200000;

            request.Headers.Add("Origin", URL_SITE);
            request.Headers.Add("Content-Encoding: gzip, deflate, br");
            request.Headers.Add("Accept-Encoding: gzip, deflate, br");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            string content = "";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream != null)
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
                }
            }

            return content;
        }
        public void GetToken(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(URL_INIT));
            request.Referer = url;
            request.ContentType = CONTENT_TYPE;
            request.UserAgent = _userAgent;
            request.CookieContainer = _cc;
            request.Timeout = 200000;

            request.Headers.Add("Origin", URL_SITE);
            request.Headers.Add("Accept-Encoding: gzip, deflate, br");
            request.Headers.Add("Content-Encoding: gzip, deflate, br");

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    if (dataStream != null)
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string content = reader.ReadToEnd();
                            _tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
                        }
                    }
                }
            }
        }
        public DataResponse LoadData(string body, string urlReferer, string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(new Uri(url));
            request.Method = "POST";
            request.ContentType = CONTENT_TYPE;
            request.UserAgent = _userAgent;
            request.Referer = urlReferer;
            request.CookieContainer = _cc;
            request.Timeout = 200000;

            request.Headers.Add("Origin", URL_SITE);
            request.Headers.Add("Accept-Encoding: gzip, deflate, br");
            request.Headers.Add("Content-Encoding: gzip, deflate, br");
            request.Headers.Add("X-Dominfo-CSRF", _tokenResponse.Response.Time + "-" + _tokenResponse.Response.Hash);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write("data=" + body);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream dataStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        return JsonConvert.DeserializeObject<DataResponse>(reader.ReadToEnd());
                    }
                }
            }
        }
        public async Task<T> GetListInfoAsync<T>(string content, string selector)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(content));

            var d = document.QuerySelector(selector);

            return JsonConvert.DeserializeObject<T>(Base.Base64Decode(d.GetAttribute("data-info")));
        }
    }
}