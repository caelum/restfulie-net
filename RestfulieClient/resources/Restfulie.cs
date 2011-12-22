using RestfulieClient.features;
using RestfulieClient.http;
using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClient.resources
{
    public class Restfulie
    {
        public IRemoteResourceService EntryPointService { get; private set; }

        private Restfulie(IRemoteResourceService service)
        {
            EntryPointService = service;
        }

        private Restfulie(string uri, IRequestDispatcher dispatcher)
        {
            EntryPointService = new EntryPointService(uri, dispatcher);
        }

        private Restfulie(string uri) : this (uri, new DefaultRequestDispatcher()) { }


        public static IRemoteResourceService At(string uri)
        {
            return new Restfulie(uri).EntryPointService;
        }

        public static IRemoteResourceService At(string uri, IRequestDispatcher dispatcher, bool useDefaultFeatures = true)
        {
            IRemoteResourceService service = new Restfulie(uri, dispatcher).EntryPointService;

            if (useDefaultFeatures)
                service = service.With(new FollowRedirects());

            return service;
        }

        public static IRemoteResourceService At(IRemoteResourceService service)
        {
            return new Restfulie(service).EntryPointService;
        }
    }
}
