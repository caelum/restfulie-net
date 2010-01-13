using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using RestfulieClient.service;
using System.Dynamic;

namespace RestfulieClient.resources
{
    public class EntryPointService : DynamicObject
    {
        private string entryPointURI = "";
        private string contentType; 
        private string accepts;

        public IRemoteResourceService RemoteResourceService { get; private set; }

        public EntryPointService(string uri, IRemoteResourceService remoteService)
        {
            this.entryPointURI = uri;
            this.RemoteResourceService = remoteService;
        }

        public dynamic As(string contentType)
        {
            this.contentType = contentType;
            this.accepts = contentType;
            return this;
        }

        public dynamic Get()
        {
            if (string.IsNullOrEmpty(this.entryPointURI))
                throw new ArgumentNullException("There is no uri defined. Use the At() method for to define the uri.");
            return this.FromXml(this.entryPointURI);
        }

        private dynamic FromXml(string uri)
        {
            HttpRemoteResponse response = (HttpRemoteResponse)this.RemoteResourceService.GetResourceFromXml(uri);
            return new DynamicXmlResource(response, this.RemoteResourceService);
        }
    }
}
