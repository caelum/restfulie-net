using System;
using System.Net;
using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClient.features
{
    public class RetryWhenUnavailable : IRequestFeature
    {
        public HttpRemoteResponse Process(RequestChain chain, Request request, string verb, Uri uri, string content) {
            var response = chain.Next(request, verb, uri, content);

            if (ShouldRetry(response))
                response = chain.Next(request, verb, uri, content);

            return response;
        }

        protected virtual bool ShouldRetry(HttpRemoteResponse response) {
            return response.StatusCode == HttpStatusCode.ServiceUnavailable;
        }
    }
}
