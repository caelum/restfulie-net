using System;
using RestfulieClient.service;
using System.Dynamic;
using System.Net;

namespace RestfulieClient.resources
{
    public class EntryPointService : IRemoteResourceService
    {
        private string entryPointURI = "";
        private string contentType;
        private string accept;

        private RestfulieHttpVerbDiscovery httpVerbDiscovery = new RestfulieHttpVerbDiscovery();

        public EntryPointService(string uri)
        {
            this.entryPointURI = uri;
        }

        public dynamic As(string contentType)
        {
            this.contentType = contentType;
            this.accept = contentType;
            return this;
        }

        public dynamic Get()
        {
            if (string.IsNullOrEmpty(this.entryPointURI))
                throw new ArgumentNullException("There is no uri defined. Use the At() method for to define the uri.");
            return this.FromXml(this.entryPointURI);
        }

        private dynamic FromXml(string uri)
        {
            dynamic response = this.GetResourceFromWeb(uri);
            //todo - criar um enum para MediaType
            if (response.ContentType.Equals("application/xml"))
            {
                return new DynamicXmlResource(response, this);
            }
            else
            {
                throw new InvalidOperationException("unsupported media type {0}", response.ContentType);
            }
        }

        public object Execute(string uri, string transitionName)
        {
            string httpVerb = httpVerbDiscovery.GetHttpVerbByTransitionName(transitionName);
            return InvokeRemoteUri(uri, httpVerb);
        }

        public object GetResourceFromWeb(string uri)
        {
            return this.InvokeRemoteUri(uri, "get");
        }

        private object InvokeRemoteUri(string uri, string httpVerb)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            try
            {
                request.Method = httpVerb;
                if (!accept.Equals(""))
                {
                    request.Accept = accept;
                    request.ContentType = contentType;
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return HttpRemoteResponseFactory.GetRemoteResponse(response);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("An error occurred while connecting to the resource in url {0} with message {1}.", uri, ex.Message), ex);
            }
        }

    }
}
