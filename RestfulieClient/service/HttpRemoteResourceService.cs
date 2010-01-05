using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestfulieClient.service;
using System.Net;
using System.IO;

namespace RestfulieClient.resources
{
    public class HttpRemoteResourceService : IRemoteResourceService
    {
        private RestfulieHttpVerbDiscovery httpVerbDiscovery = new RestfulieHttpVerbDiscovery();
        
        public object Execute(string uri,string transitionName)
        {
            string httpVerb = httpVerbDiscovery.GetHttpVerbByTransitionName(transitionName);
            return InvokeRemoteUri(uri, httpVerb);
        }

        public object GetResourceFromXml(string uri)
        {
            return ((HttpRemoteResourceResponse)this.InvokeRemoteUri(uri, "get"));
        }        

        private object InvokeRemoteUri(string uri, string httpVerb)
        {
            WebRequest request = WebRequest.Create(uri);
            try
            {
                request.Method = httpVerb;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());                
                string xml = reader.ReadToEnd();
                return new HttpRemoteResourceResponse() { XmlRepresentation = xml, WebResponse = response };
                //response.StatusDescription;
                //return xml;

            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("An error occurred while connecting to the resource in url {0} with message {1}.", uri, ex.Message), ex);
            }
        }
    }
}
