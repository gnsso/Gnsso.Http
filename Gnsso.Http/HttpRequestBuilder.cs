using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Gnsso.Http
{
    class HttpRequestBuilder : IHttpRequestBuilder
    {
        private string uri;

        private IList<HttpRequestBuilderAction> actions;

        public int Timeout { get; set; } = 100000;
        public int RetriesAfterTimeout { get; set; } = 1;

        public HttpRequestBuilder(string uri, string method, string userAgent, CookieContainer cookies)
        {
            this.uri = uri;
            SetTask(request => request.CookieContainer = cookies, -2);
            SetTask(request => request.UserAgent = userAgent, -1);
            SetTask(request => request.Method = method, -1);
        }

        #region Priority = -1

        public IHttpRequestBuilder Header(string name, string value)
        {
            return SetTask(request => request.Headers.Add(name, value), -1);
        }

        public IHttpRequestBuilder Accept(string accept)
        {
            return SetTask(request => request.Accept = accept, -1);
        }

        public IHttpRequestBuilder AcceptEncoding(DecompressionMethods decompression)
        {
            return SetTask(request => request.AutomaticDecompression = decompression, -1);
        }

        public IHttpRequestBuilder ContentType(string contentType)
        {
            return SetTask(request => request.ContentType = contentType, -1);
        }

        public IHttpRequestBuilder Referer(string referer)
        {
            return SetTask(request => request.Referer = new Uri(new Uri(uri), referer).AbsoluteUri, -1);
        }

        public IHttpRequestBuilder UserAgent(string userAgent)
        {
            return SetTask(request => request.UserAgent = userAgent, -1);
        }

        public IHttpRequestBuilder KeepAlive(bool keepAlive)
        {
            return SetTask(request => request.KeepAlive = keepAlive, -1);
        }

        #endregion

        #region Priority = 0

        public IHttpRequestBuilder Query(string query, bool inline)
        {
            if (inline)
            {
                uri += $"?{query}";
                return this;
            }
            else
            {
                return SetTask(request =>
                {
                    var data = Encoding.ASCII.GetBytes(query);
                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream()) stream.Write(data, 0, data.Length);
                }, 0);
            }
        }

        #endregion

        public void Response()
        {
            Response(out HttpWebResponse response);
            response.Dispose();
        }

        public void Response(out HttpWebResponse response)
        {
            var request = PrepareRequest();
            var isSuccess = false;
            var retries = RetriesAfterTimeout;
            do
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    isSuccess = true;
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.Timeout && (--retries > 0))
                    {
                        isSuccess = false;
                        response = null;
                        System.Threading.Thread.Sleep(1000);
                    }
                    else throw e;
                }
            } while (!isSuccess);
        }

        public void Response(out string result)
        {
            Response(s => s, out result);
        }

        public void Response<T>(Func<string, T> parser, out T result)
        {
            Response(out HttpWebResponse response);
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
                result = parser(reader.ReadToEnd());
        }

        private IHttpRequestBuilder SetTask(Action<HttpWebRequest> action, int priority)
        {
            (actions ?? (actions = new List<HttpRequestBuilderAction>())).Add(new HttpRequestBuilderAction
            {
                Action = action,
                Priority = priority
            });
            return this;
        }

        private HttpWebRequest PrepareRequest()
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            foreach (Action<HttpWebRequest> action in actions.OrderBy(o => o.Priority)) action(request);
            request.Timeout = Timeout;
            return request;
        }
    }
}
