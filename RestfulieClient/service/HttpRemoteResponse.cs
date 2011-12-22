using System;
using System.Linq;
using System.Net;
using System.Dynamic;
using System.Collections.Generic;

namespace RestfulieClient.service
{
    public class HttpRemoteResponse : DynamicObject
    {        
        public string Content { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public Dictionary<string,string> Headers { get; private set; }

        public HttpRemoteResponse(HttpStatusCode statusCode, Dictionary<string, string> headers, string content)
        {
            if (headers == null)
                throw new ArgumentNullException("headers");

            StatusCode = statusCode;
            Headers = headers.ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase); // make a copy for better comparisons
            Content = content;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string headerValue = "";
            if (Headers.TryGetValue(binder.Name.Replace("_", "-"), out headerValue))
            {
                result = headerValue;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public bool HasNoContent()
        {
            return String.IsNullOrEmpty(Content);
        }
    }
}

