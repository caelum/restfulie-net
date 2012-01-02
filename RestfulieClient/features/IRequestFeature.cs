using System;
using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClient.features
{
    public interface IRequestFeature
    {
        HttpRemoteResponse Process(RequestChain chain, Request request, string verb, Uri uri, string content);
    }
}
