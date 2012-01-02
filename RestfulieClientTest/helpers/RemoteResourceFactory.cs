using System;
using System.Collections.Generic;
using RestfulieClient.resources;
using RestfulieClient.service;
using System.Net;

namespace RestfulieClientTests.helpers
{
    public class RemoteResourceFactory
    {

        public static IRemoteResourceService GetRemoteResource(string fileName)
        {
            return new RemoteResourceMock(fileName);
        }

        public class RemoteResourceMock : IRemoteResourceService
        {
            private string fileName;

            public RemoteResourceMock(string fileName)
            {
                this.fileName = fileName;
            }

            public object Execute(string uri, string transitionName)
            {
                return GetMediaTypeXMLResponse(uri);
            }

            public object GetResourceFromWeb(string uri)
            {
                return GetMediaTypeXMLResponse(uri);
            }

            public dynamic Get()
            {
                if (this.fileName == null || this.fileName.Equals(""))
                    throw new ArgumentNullException();
                else
                    return GetMediaTypeXMLResponse(fileName);
            }

            public dynamic Create(string content)
            {
                return new DynamicXmlResource(CreateRemoteResponse(content));
            }


            private static DynamicXmlResource GetMediaTypeXMLResponse(string uri)
            {
                return new DynamicXmlResource(GetHttpRemoteResponseFake(uri));
            }


            private static HttpRemoteResponse GetHttpRemoteResponseFake(string fileName)
            {
                string content = GetContentFromFile(fileName);
                return CreateRemoteResponse(content);
            }

            private static HttpRemoteResponse CreateRemoteResponse(string content)
            {
                HttpRemoteResponse response = new HttpRemoteResponse(HttpStatusCode.OK,
                    new Dictionary<string, string>(), content);
                return response;
            }

            private static string GetContentFromFile(string fileName)
            {
                string content = "";
                if (fileName != "" && fileName != "")
                    content = new LoadDocument().GetDocumentContent(fileName);
                return content;
            }


        }
    }
}
