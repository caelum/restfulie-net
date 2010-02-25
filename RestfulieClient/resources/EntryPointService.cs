using System;
using System.Net;
using RestfulieClient.service;
using System.Text;
using System.IO;
using System.Net.Mime;

namespace RestfulieClient.resources
{
    public class EntryPointService : IRemoteResourceService
    {
        private string entryPointURI = "";
        private string contentType = "";
        private string accepts = "";

        private RestfulieHttpVerbDiscovery httpVerbDiscovery = new RestfulieHttpVerbDiscovery();

        public EntryPointService(string uri)
        {
            this.entryPointURI = uri;
        }

        public dynamic As(string contentType)
        {
            this.contentType = contentType;
            this.accepts = contentType;
            return this;
        }

        public dynamic Accepts(string acceptType)
        {
            this.accepts = acceptType;
            return this;
        }

        public dynamic Get()
        {
            if (string.IsNullOrEmpty(this.entryPointURI))
                throw new ArgumentNullException("There is no uri defined. Use the At() method for to define the uri.");
            HttpWebResponse response = (HttpWebResponse)this.FromWeb(this.entryPointURI);
            return ParseGetResponse(response);
        }

        private dynamic ParseGetResponse(HttpWebResponse res)
        {
            dynamic response = HttpRemoteResponseFactory.GetRemoteResponse(res);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                return ParseGetOkResponse(res, response);
            }
            else
                return response;
        }

        private dynamic ParseGetOkResponse(HttpWebResponse res, dynamic response)
        {          
            if (res.ContentType.Contains("application/xml"))
            {
                return new DynamicXmlResource(response, this);
            }
            else
            {
                throw new InvalidOperationException("unsupported media type: " + res.ContentType);
            }
        }

        public dynamic Create(string content)
        {
            return InvokeRemoteUri(this.entryPointURI, "post", content);
        }

        public dynamic ParsePostResponse(HttpWebResponse res, string content)
        {
            dynamic response = HttpRemoteResponseFactory.GetRemoteResponse(res);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                this.accepts = "application/xml";                               
                return FromWeb(response.Location);
            }
            else 
                return response;    
        }
        /*
              def parse_post_response(response, content)
                code = response.code
                if code=="301" && @type.follows.moved_permanently? == :all
                  remote_post_to(response["Location"], content)
                elsif code=="201"
                  from_web(response["Location"], "Accept" => "application/xml")
                else
                  response
                end
              end                      
        */
        private dynamic FromWeb(string uri)
        {
            WebResponse response = this.InvokeRemoteUri(uri, "get");
            return response;
        }

        public object Execute(string uri, string transitionName)
        {
            string httpVerb = httpVerbDiscovery.GetHttpVerbByTransitionName(transitionName);
            return InvokeRemoteUri(uri, httpVerb);
        }


        private HttpWebResponse InvokeRemoteUri(string uri, string httpVerb, string content = "")
        {
            Uri requestUri = new Uri(this.entryPointURI);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            try
            {
                request.Method = httpVerb;
                if (!accepts.Equals(""))
                {
                    request.Accept = accepts;
                    request.ContentType = contentType;
                }
                if (!content.Equals(""))
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(content);
                    request.ContentLength = byteArray.Length;
                    Stream bodyStream = request.GetRequestStream();
                    bodyStream.Write(byteArray, 0, byteArray.Length);
                    bodyStream.Close();
                }
                return (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("An error occurred while connecting to the resource in url {0} with message {1}.", uri, ex.Message), ex);
            }
        }

    }
}
