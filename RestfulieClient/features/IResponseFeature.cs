using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClient.features
{
    public interface IResponseFeature
    {
        HttpRemoteResponse Process(ResponseChain chain, HttpRemoteResponse response);
    }
}
