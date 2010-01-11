using System.Net;
using System.Dynamic;
using System.Collections.Generic;
using System.Xml.Linq;

namespace RestfulieClient.service
{
    public class HttpRemoteResponse : DynamicObject
    {        
        public string Content { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
        public Dictionary<string,string> Headers { get; private set; }

        public HttpRemoteResponse(HttpStatusCode statusCode, Dictionary<string, string> headers, string content)
        {
            this.StatusCode = statusCode;
            this.Headers = headers;
            this.Content = content;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string headerValue = "";
            if (Headers.TryGetValue(binder.Name.ToUpper(), out headerValue))
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
            return Content.Equals("");
        }
    }
}

