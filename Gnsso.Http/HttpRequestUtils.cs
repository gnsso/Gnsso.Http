using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gnsso.Http
{
    internal static class HttpRequestUtils
    {
        public static void SetupRequest(ref HttpWebRequest request, Uri uri, string method, 
            string accept = null,
            string contentType = null)
        {
            var cookies = request.CookieContainer;
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = cookies;
            request.Method = method;
            if (accept != null) request.Accept = accept;
            if (contentType != null) request.ContentType = contentType;
        }
    }
}
