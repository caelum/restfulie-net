using System.Collections.Generic;
using RestfulieClient.features;
using RestfulieClient.service;

namespace RestfulieClient.request
{
    public class ResponseChain
    {
        private readonly IRemoteResourceService _service;
        private readonly IEnumerator<IResponseFeature> _current;

        public IRemoteResourceService Service {
            get { return _service; }
        }

        public ResponseChain(IRemoteResourceService service, IEnumerable<IResponseFeature> features) {
            _service = service;
            _current = features.GetEnumerator();
        }

        public void Reset() {
            _current.Reset();
        }

        public HttpRemoteResponse Next(HttpRemoteResponse response) {
            return _current.MoveNext() ? _current.Current.Process(this, response) : response;
        }
    }
}
