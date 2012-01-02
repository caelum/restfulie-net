using System.Collections.Generic;
using RestfulieClient.features;
using RestfulieClient.request;
using RestfulieClient.resources;

namespace RestfulieClient.service
{
    public delegate void RestfulieCallback(IResource resource, object state);

    public interface IRemoteResourceService
    {
        /// <summary>
        /// Returns whether the service has been configured to run asynchronously
        /// </summary>
        bool IsAsynch { get; }

        /// <summary>
        /// Returns the registered dispatcher for this service
        /// </summary>
        IRequestDispatcher Dispatcher { get; }

        /// <summary>
        /// Returns the registered request headers
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Specifies the media type that the server should attempt to return
        /// </summary>
        IRemoteResourceService Accepts(string mediaType);

        /// <summary>
        /// Specifies the media type that will be sent to the server
        /// </summary>
        IRemoteResourceService As(string mediaType);

        /// <summary>
        /// Shortcut to specify Accept and As together
        /// </summary>
        IRemoteResourceService Handling(string mediaType);

        /// <summary>
        /// Flags the service to dispatch asynchronously and return results to the provided callback
        /// </summary>
        /// <returns></returns>
        IRemoteResourceService Asynch(RestfulieCallback callback, object state);

        /// <summary>
        /// Allows a custom header to be set for requests
        /// </summary>
        IRemoteResourceService With(string name, string value);

        /// <summary>
        /// Allows a feature to be added to the request stack
        /// </summary>
        IRemoteResourceService With(IRequestFeature requestFeature);

        /// <summary>
        /// Allows a feature to be added to the response stack
        /// </summary>
        IRemoteResourceService With(IResponseFeature responseFeature);

        /// <summary>
        /// Executes a GET request against the given uri
        /// </summary>
        object Execute(string uri);

        /// <summary>
        /// Executes a request against the given uri based on the transition name provided
        /// </summary>
        object Execute(string uri, string transitionName);
        
        /// <summary>
        /// Executes a request with content against the given uri based on the transition name provided
        /// </summary>
        object Execute(string uri, string transitionName, string content);
        
        dynamic Get();
        dynamic Create(string content);
    } 
}
