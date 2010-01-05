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
            HttpRemoteResourceResponse response = (HttpRemoteResourceResponse)this.RemoteResourceService.GetResourceFromXml(uri); 
            XElement element = XElement.Parse(response.XmlRepresentation);
            return new DynamicXmlResource(element) {  remoteResourceService = RemoteResourceService, WebResponse = response.WebResponse};
        }
    }
}
