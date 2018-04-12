using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gnsso.Http
{
    public interface IHttpRequestBuilder
    {
        IHttpRequestBuilder Query(string query, bool inline);
        IHttpRequestBuilder Accept(string accept);
        IHttpRequestBuilder ContentType(string contentType);
        IHttpRequestBuilder Referer(string referer);
        IHttpRequestBuilder UserAgent(string userAgent);
        IHttpRequestBuilder KeepAlive(bool keepAlive);
        IHttpRequestBuilder Header(string name, string value);
        void Response();
        void Response(out HttpWebResponse response);
        void Response(out string result);
        void Response<T>(Func<string, T> parser, out T result);
    }
}
