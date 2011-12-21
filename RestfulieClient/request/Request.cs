using System;
using RestfulieClient.features;
using RestfulieClient.service;

namespace RestfulieClient.request
{
    public class Request
    {
        private readonly IRequestDispatcher _dispatcher;
        private readonly RequestStack _stack;

        public IRequestDispatcher Dispatcher {
            get { return _dispatcher; }
        }

        public Request(IRemoteResourceService service, IRequestDispatcher dispatcher) {
            _dispatcher = dispatcher;
            _stack = new RequestStack(service);
        }

        public void AddFeature(IRequestFeature requestFeature) {
            _stack.AddFeature(requestFeature);
        }

        public void AddFeature(IResponseFeature responseFeature) {
            _stack.AddFeature(responseFeature);
        }

        public virtual HttpRemoteResponse Process(Uri uri, string verb, string content) {
            return _stack.Process(this, verb, uri, content);
        }
    }
}
