using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestfulieClient.service;
using System.Dynamic;

namespace RestfulieClient.resources
{
    public class Restfulie : DynamicObject
    {
        public EntryPointService EntryPointService { get; private set; }

        private Restfulie(EntryPointService service)
        {
            this.EntryPointService = service;
        }

        private Restfulie(string uri)
        {
            this.EntryPointService = new EntryPointService(uri, new HttpRemoteResourceService());
        }

        public static dynamic At(string uri)
        {
            Restfulie entryPoint = new Restfulie(uri);
            return entryPoint.EntryPointService;
        }

        public static dynamic At(EntryPointService service)
        {
            Restfulie entryPoint = new Restfulie(service);
            return entryPoint.EntryPointService;
        }
    }
}
