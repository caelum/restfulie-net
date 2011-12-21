using System;
using RestfulieClient.service;

namespace RestfulieClient.request
{
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Instructs the dispatcher to process the given request based on it's current configuration
        /// </summary>
        /// <param name="service">The service that called the dispatcher</param>
        /// <param name="verb">The HTTP verb to use for making the request</param>
        /// <param name="uri">The URI to use for making the request</param>
        /// <param name="content">The text to include in the request body</param>
        /// <returns>The result of the request (including status code and content), will be null if using an asynch callback</returns>
        HttpRemoteResponse Process(IRemoteResourceService service, string verb, Uri uri, string content);
    }
}
