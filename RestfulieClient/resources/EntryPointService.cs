using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RestfulieClient.resources
{
    public class EntryPointService
    {
        public IRemoteResourceService RemoteResourceService { get; set; }

        public dynamic FromXml(string uri)
        { 
            string xml = this.RemoteResourceService.GetResourceFromXml(uri);
            XElement element = XElement.Parse(xml);
            return new DynamicXmlResource(element) {  remoteResourceService = RemoteResourceService};
        }
    }
}
