using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimplisafeSharp
{
    public class SimplisafeSharp
    {
        public enum StatusType { Off, Home, Away };
        private List<Cookie> _cookieCollection = new List<Cookie>();
        private readonly string _baseUrl = ConfigurationManager.AppSettings["SimplisafeBaseUrl"];
        private string UserId;

        public SimplisafeResponse Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            using (var client = new CookieAwareWebClient())
            {
                byte[] response = client.UploadValues($"{_baseUrl}login", new NameValueCollection()
                {
                    { "name", username },
                    { "pass", password },
                    { "device_name", "SimplisafeSharp" },
                    { "device_uuid", "" },
                    { "version", "" },
                    { "no_persist", "" },
                    { "XDEBUG_SESSION_START", "" },
                });

                foreach (Cookie item in client.ResponseCookies)
                {
                    _cookieCollection.Add(item);
                }

                string result = Encoding.UTF8.GetString(response);
                dynamic d = Newtonsoft.Json.Linq.JObject.Parse(result);

                if (d.return_code != 3)
                    return null;

                var responseObj = new SimplisafeResponse
                {
                    SessionId = d.session,
                    UserId = d.uid
                };
                UserId = d.uid;


                return responseObj;
            }
        }

        public async Task<string> GetLocations(SimplisafeResponse userData)
        {
            try
            {
                var baseAddress = new Uri($"{_baseUrl}");
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    foreach (Cookie item in _cookieCollection)
                    {
                        cookieContainer.Add(item);
                    }

                    var result = client.PostAsync($"{UserId}/locations", null).Result;
                    result.EnsureSuccessStatusCode();

                    var contents = await result.Content.ReadAsStringAsync();
                    return contents;

                }

            }
            catch (Exception ex)
            {
                var something = ex;
            }

            return null;
        }

        public async Task<string> SetStatus(StatusType status, int locationId)
        {
            try
            {
                var baseAddress = new Uri($"{_baseUrl}");
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    foreach (Cookie item in _cookieCollection)
                    {
                        cookieContainer.Add(item);
                    }

                    var content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("state", status.ToString().ToLower()),
                        new KeyValuePair<string, string>("mobile", "1")
                    });

                    var result = client.PostAsync($"{UserId}/sid/{locationId}/set-state", content).Result;
                    result.EnsureSuccessStatusCode();

                    return await result.Content.ReadAsStringAsync();

                }

            }
            catch (Exception ex)
            {
                var something = ex;
            }
            return null;
        }
    }

    public class CookieAwareWebClient : WebClient
    {
        public CookieAwareWebClient()
        {
            CookieContainer = new CookieContainer();
            this.ResponseCookies = new CookieCollection();
        }

        public CookieContainer CookieContainer { get; private set; }
        public CookieCollection ResponseCookies { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = (HttpWebResponse)base.GetWebResponse(request);
            this.ResponseCookies = response.Cookies;
            return response;
        }
    }
}
