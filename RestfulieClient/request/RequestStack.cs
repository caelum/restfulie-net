using System;
using System.Collections.Generic;
using RestfulieClient.features;
using RestfulieClient.service;

namespace RestfulieClient.request
{
    public class RequestStack : IRequestFeature
    {
        private readonly IRemoteResourceService _service;
        private readonly IList<IRequestFeature> _requestFeatures = new List<IRequestFeature>();
        private readonly IList<IResponseFeature> _responseFeatures = new List<IResponseFeature>();

        public RequestStack(IRemoteResourceService service) {
            _service = service;
        }

        public void AddFeature(IRequestFeature requestFeature) {
            _requestFeatures.Add(requestFeature);
        }

        public void AddFeature(IResponseFeature responseFeature) {
            _responseFeatures.Add(responseFeature);
        }

        public HttpRemoteResponse Process(Request request, string verb, Uri uri, string content) {
            _requestFeatures.Add(this);

            return new RequestChain(_requestFeatures).Next(request, verb, uri, content);
        }

        public virtual HttpRemoteResponse Process(RequestChain chain, Request request, string verb, Uri uri, string content) {
            return new ResponseChain(_service, _responseFeatures)
                .Next(_service.Dispatcher.Process(_service, verb, uri, content));
        }
    }
}
