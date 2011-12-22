using System;
using RestfulieClient.service;

namespace RestfulieClient.resources
{
    public class EmptyResource : IResource
    {
        public HttpRemoteResponse WebResponse { get; private set; }

        public bool IsEmpty { get { return true; } }

        public EmptyResource(HttpRemoteResponse webResponse) {
            WebResponse = webResponse;
        }

        public bool HasLink(string rel) {
            return false;
        }

        public IResource Follow(string rel, string content) {
            throw new NotImplementedException("Empty resource does not have anything to follow");
        }

        public T As<T>() where T : class {
            throw new NotImplementedException("Empty resource cannot be converted");
        }

        public T[] AsMany<T>() where T : class {
            throw new NotImplementedException("Empty resource cannot be converted");
        }
    }
}
