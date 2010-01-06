using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestfulieClient.resources
{
    public class Restfulie
    {
        private string entryPointURI = "";
        public EntryPointService EntryPointService { get; set; }

        private Restfulie(string uri)
        {
            this.EntryPointService = new EntryPointService() { RemoteResourceService = new HttpRemoteResourceService() };
            this.entryPointURI = uri;
        }

        public static Restfulie At(string uri)
        {
            Restfulie entryPoint = new Restfulie(uri);
            return entryPoint;
        }

        public static Restfulie At(string uri, EntryPointService service)
        {
            Restfulie entryPoint = new Restfulie(uri);
            return entryPoint;
        }

        public dynamic Get()
        {
            if (string.IsNullOrEmpty(this.entryPointURI))
                throw new ArgumentNullException("There is no uri defined. Use the At() method for to define the uri.");
            return this.EntryPointService.FromXml(this.entryPointURI);
        }
        

    }
}
