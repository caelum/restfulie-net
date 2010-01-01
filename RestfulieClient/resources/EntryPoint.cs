using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestfulieClient.resources
{
    public class EntryPoint
    {
        private string entryPointURI = "";
        public EntryPointService EntryPointService { get; set; }

        public EntryPoint()
        {
            this.EntryPointService = new EntryPointService() { RemoteResourceService = new HttpRemoteResourceService() };
        }

        public EntryPoint At(string uri)
        {
            this.entryPointURI = uri;
            return this;
        }

        public dynamic Get()
        {
            if (string.IsNullOrEmpty(this.entryPointURI))
                throw new ArgumentNullException("There is no uri defined. Use the At() method for to define the uri.");
            return this.EntryPointService.FromXml(this.entryPointURI);
        }
        

    }
}
