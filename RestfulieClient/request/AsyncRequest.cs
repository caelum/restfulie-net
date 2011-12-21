using System;
using System.Threading;
using RestfulieClient.service;

namespace RestfulieClient.request
{
    public delegate void AsynchRequestCallback(HttpRemoteResponse response, object state);

    public class AsynchRequest
    {
        private readonly Request _request;
        private readonly AsynchRequestCallback _callback;

        public AsynchRequest(Request request, AsynchRequestCallback callback) {
            _request = request;
            _callback = callback;
        }

        private void AsynchCallback(HttpRemoteResponse response, object state) {
            // bubble it up
            var callback = _callback;

            if (callback != null)
                callback(response, state);
        }

        private void AsynchWorker(object rawState) {
            dynamic state = rawState;

            HttpRemoteResponse response = _request.Process(
                state.Uri, state.Verb, state.Payload);

            AsynchCallback(response, rawState);
        }

        public void Process(Uri uri, string verb, object payload = null) {
            var state = new { Uri = uri, Verb = verb, Payload = payload };

            ThreadPool.QueueUserWorkItem(AsynchWorker, state);
        }
    }
}
