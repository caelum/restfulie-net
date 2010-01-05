using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RestfulieClient.service
{
    public class HttpRemoteResourceResponse
    {
        public string XmlRepresentation { get; set; }
        public HttpWebResponse WebResponse { get; set; }
    }
}
