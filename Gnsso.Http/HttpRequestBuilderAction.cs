using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Gnsso.Http
{
    internal class HttpRequestBuilderAction
    {
        public Action<HttpWebRequest> Action { get; set; }
        public int Priority { get; set; }

        public static explicit operator Action<HttpWebRequest>(HttpRequestBuilderAction a) => a.Action;
    }
}
