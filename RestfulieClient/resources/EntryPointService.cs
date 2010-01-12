using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using RestfulieClient.service;

namespace RestfulieClient.resources
{
    public class EntryPointService
    {
        public IRemoteResourceService RemoteResourceService { get; set; }

        public dynamic FromXml(string uri)
        {
            HttpRemoteResponse response = (HttpRemoteResponse)this.RemoteResourceService.GetResourceFromXml(uri);
            return new DynamicXmlResource(response,this.RemoteResourceService);
        }
    }
}
