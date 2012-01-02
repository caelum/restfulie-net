using System;
using System.Collections.Generic;
using RestfulieClient.features;
using RestfulieClient.service;

namespace RestfulieClient.request
{
    public class RequestChain
    {
        private readonly IEnumerator<IRequestFeature> _current;

        public RequestChain(IEnumerable<IRequestFeature> features) {
            _current = features.GetEnumerator();
        }

        public HttpRemoteResponse Next(Request request, string verb, Uri uri, string content) {
            return _current.MoveNext() ? _current.Current.Process(this, request, verb, uri, content) : null;
        }
    }
}
