using System;
using System.Net;
using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClient.features
{
    public class FollowRedirects : IResponseFeature
    {
        public HttpRemoteResponse Process(ResponseChain chain, HttpRemoteResponse response) {
            response = chain.Next(response);

            if (!ShouldRedirect(response))
                return response;

            string uri;
            
            if (response.Headers.TryGetValue("Location", out uri) && !String.IsNullOrWhiteSpace(uri)) {
                chain.Reset();

                response = chain.Service.Dispatcher.Process(chain.Service, "GET", new Uri(uri), null);
                response = chain.Next(response);
            }

            return response;
        }

        protected bool ShouldRedirect(HttpRemoteResponse response) {
            return (int)response.StatusCode / 100 == 3 || (
                   response.StatusCode == HttpStatusCode.Created && response.HasNoContent());
        }
    }
}
