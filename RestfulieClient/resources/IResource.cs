using System;
using RestfulieClient.service;

namespace RestfulieClient.resources
{
    public interface IResource
    {
        HttpRemoteResponse WebResponse { get; }

        /// <summary>
        /// Indicates whether this resource is considered empty (even if content length > 0)
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Checks whether the specified link exists in the response
        /// </summary>
        /// <param name="rel">The expected rel of the link to lookup</param>
        bool HasLink(string rel);

        /// <summary>
        /// Causes the resource to follow the specified link and issue a new request.
        /// It is expected that an <see cref="ArgumentException"/> will be thrown if the link does not exist.
        /// </summary>
        /// <param name="rel">The rel of the link to follow</param>
        /// <param name="content">The content to include in the request body</param>
        /// <returns>The resource result of the new request</returns>
        IResource Follow(string rel, string content);

        /// <summary>
        /// Converts the dynamic resource to a typed object
        /// </summary>
        /// <typeparam name="T">The type of the class to convert to</typeparam>
        /// <returns>The converted object</returns>
        T As<T>() where T : class;

        /// <summary>
        /// Converts the dynamic collection resource to an array of typed objects
        /// </summary>
        /// <typeparam name="T">The type of the class to convert to</typeparam>
        /// <returns>The converted objects</returns>
        T[] AsMany<T>() where T : class;
    }
}
