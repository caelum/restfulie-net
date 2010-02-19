using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestfulieClient.service;
using System.Dynamic;

namespace RestfulieClient.resources
{
    public class Restfulie
    {
        public IRemoteResourceService EntryPointService { get; private set; }

        private Restfulie(IRemoteResourceService service)
        {
            this.EntryPointService = service;
        }

        private Restfulie(string uri)
        {
            this.EntryPointService = new EntryPointService(uri);
        }

        public static IRemoteResourceService At(string uri)
        {
            Restfulie entryPoint = new Restfulie(uri);
            return entryPoint.EntryPointService;
        }

        public static IRemoteResourceService At(IRemoteResourceService service)
        {
            Restfulie entryPoint = new Restfulie(service);
            return entryPoint.EntryPointService;
        }
    }
}
