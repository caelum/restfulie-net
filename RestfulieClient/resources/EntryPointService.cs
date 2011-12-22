using System;
using System.Collections.Generic;
using System.Net;
using RestfulieClient.features;
using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClient.resources
{
    public class EntryPointService : IRemoteResourceService
    {
        private readonly string _entryPointUri = "";
        private readonly IRequestDispatcher _dispatcher;
        private readonly IList<IRequestFeature> _requestFeatures = new List<IRequestFeature>();
        private readonly IList<IResponseFeature> _responseFeatures = new List<IResponseFeature>();
        private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly RestfulieHttpVerbDiscovery _httpVerbDiscovery = new RestfulieHttpVerbDiscovery();
        private bool _asych;
        private RestfulieCallback _clientCallback;
        private object _clientCallbackState;

        public IRequestDispatcher Dispatcher {
            get { return _dispatcher; }
        }

        public bool IsAsynch {
            get { return _asych; }
        }

        public IDictionary<string, string> Headers {
            get { return _headers; }
        }

        public EntryPointService(string uri, IRequestDispatcher dispatcher) {
            _entryPointUri = uri;
            _dispatcher = dispatcher;
        }

        private IResource CreateResource(string contentType, HttpRemoteResponse response) {
            if (contentType.IndexOf("application/xml", StringComparison.OrdinalIgnoreCase) > -1 ||
                contentType.IndexOf("text/xml", StringComparison.OrdinalIgnoreCase) > -1)
                return new DynamicXmlResource(response, this);

            return null;
        }

        private IResource ParseResponse(HttpRemoteResponse response) {
            if (response.StatusCode >= HttpStatusCode.BadRequest ||
                response.HasNoContent() || !response.Headers.ContainsKey("Content-Type"))
                return new EmptyResource(response);

            string contentType = response.Headers["Content-Type"];
            IResource resource = CreateResource(contentType, response);

            if (resource != null)
                return resource;

            throw new InvalidOperationException("unsupported media type: " + response.Headers["Content-Type"]);
        }
        
        private void AsynchCallback(HttpRemoteResponse response, object state) {
            var callback = _clientCallback;

            if (callback != null)
                callback(ParseResponse(response), _clientCallbackState);
        }

        private IResource Process(Uri uri, string verb, string content) {
            var request = new Request(this, _dispatcher);

            foreach (var feature in _requestFeatures)
                request.AddFeature(feature);
            foreach (var feature in _responseFeatures)
                request.AddFeature(feature);

            if (_asych) {
                new AsynchRequest(request, AsynchCallback).Process(uri, verb, content);
                return null;
            }

#if SILVERLIGHT
            throw new InvalidOperationException("Silverlight must be run in async mode by calling Asynch before any executing methods (e.g. Get)");
#endif

            return ParseResponse(request.Process(uri, verb, content));
        }

        private string GetVerb(string transitionName) {
            return _httpVerbDiscovery.GetHttpVerbByTransitionName(transitionName);
        }

        public IRemoteResourceService As(string mediaType) {
            return With("Content-Type", mediaType);
        }

        public IRemoteResourceService Accepts(string mediaType) {
            return With("Accept", mediaType);;
        }

        public IRemoteResourceService Handling(string mediaType) {
            return As(mediaType).Accepts(mediaType);
        }
        
        public IRemoteResourceService Asynch(RestfulieCallback callback, object state) {
            _asych = true;
            _clientCallback = callback;
            _clientCallbackState = state;
            return this;
        }

        public IRemoteResourceService With(string name, string value) {
            _headers.Add(name, value);
            return this;
        }

        public IRemoteResourceService With(IRequestFeature requestFeature) {
            _requestFeatures.Add(requestFeature);
            return this;
        }

        public IRemoteResourceService With(IResponseFeature responseFeature) {
            _responseFeatures.Add(responseFeature);
            return this;
        }

        private IResource Process(string uri, string verb, string content) {
            return Process(new Uri(uri), verb, content);
        }

        public object Execute(string uri) {
            return Process(uri, "GET", null);
        }

        public object Execute(string uri, string transitionName) {
            return Process(uri, GetVerb(transitionName), null);
        }

        public object Execute(string uri, string transitionName, string content) {
            return Process(uri, GetVerb(transitionName), content);
        }

        public dynamic Get() {
            if (_entryPointUri == null)
                throw new InvalidOperationException("There is no uri defined. Use the At() method for to define the uri.");

            return Process(_entryPointUri, "GET", null);
        }

        public dynamic Create(string content) {
            if (_entryPointUri == null)
                throw new InvalidOperationException("There is no uri defined. Use the At() method for to define the uri.");

            return Process(_entryPointUri, "POST", content);
        }
    }
}
