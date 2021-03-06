﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gnsso.Http
{
    public abstract class HttpClient
    {
        private CookieContainer cookies;

        public CookieContainer Cookies
        {
            get => cookies;
            set => cookies = value;
        }

        public abstract string BaseUrl { get; }
        public virtual string UserAgent { get; } = "Gnsso.Http";
        public virtual int Timeout { get; } = 10000;
        public virtual int RetriesAfterTimeout { get; } = 3;

        public HttpClient()
        {
            cookies = new CookieContainer();
        }

        public IHttpRequestBuilder Get(string url)
        {
            return CreateRequestBuilder(url, "GET");
        }

        public IHttpRequestBuilder Post(string url)
        {
            return CreateRequestBuilder(url, "POST");
        }

        public IHttpRequestBuilder Options(string url)
        {
            return CreateRequestBuilder(url, "OPTIONS");
        }

        private IHttpRequestBuilder CreateRequestBuilder(string url, string method)
        {
            return new HttpRequestBuilder(new Uri(new Uri(BaseUrl), url).AbsoluteUri, method, UserAgent, cookies)
            {
                Timeout = Timeout,
                RetriesAfterTimeout = RetriesAfterTimeout
            };
        }
    }
}
