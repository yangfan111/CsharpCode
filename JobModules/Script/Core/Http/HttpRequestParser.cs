using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Core.Http
{
    public class HttpRequestParser
    {
        public NameValueCollection QueryItem { get; private set; }
        public Uri url { get; private set; }

        public HttpRequestParser(HttpListenerRequest request)
        {
            url = request.Url;
            QueryItem = request.QueryString;
            if (request.HttpMethod.Equals("POST"))
            {
                StreamReader sr = new StreamReader(request.InputStream);
                HttpUtility.QueryDecodeTo(sr.ReadToEnd(),QueryItem);
            }
        }
    }
}