using System;
using System.Collections.Generic;
using System.Net;
using Moq;
using RestfulieClient.features;
using RestfulieClient.request;
using RestfulieClient.resources;
using RestfulieClient.service;

namespace RestfulieClientTests.helpers
{
    public static class TestHelper
    {
        public static Mock<IRemoteResourceService> CreateService(string rawUri, string contentType = "application/xml") {
            var service = new Mock<IRemoteResourceService>();

            var headers = new Dictionary<string, string> {
                {"Accept", contentType},
                {"Content-Type", contentType}
            };

            service.SetupGet(s => s.Headers).Returns(headers);
            service.Setup(s => s.Execute(It.IsAny<string>()))
                .Returns<string>(u => GetDynamicResourceWithServiceFake(u, contentType));
            service.Setup(s => s.Execute(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((u, t) => GetDynamicResourceWithServiceFake(u, contentType));
            service.Setup(s => s.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string, object>((u, t, p) => GetDynamicResourceWithServiceFake(u, contentType));

            return service;
        }

        
        public static HttpRemoteResponse CreateResponse(HttpStatusCode code = HttpStatusCode.OK, Dictionary<string, string> headers = null, string content = "") {
            return new HttpRemoteResponse(code, headers ?? new Dictionary<string, string>(), content);
        }
        
        public static Mock<IRequestDispatcher> CreateDispatcher(HttpRemoteResponse response) {
            var dispatcher = new Mock<IRequestDispatcher>();

            dispatcher.Setup(d => d.Process(It.IsAny<IRemoteResourceService>(), It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns(response);
            
            return dispatcher;
        }
        
        public static Mock<IRequestFeature> CreateRequestFeature(HttpRemoteResponse response) {
            var requestFeature = new Mock<IRequestFeature>();

            requestFeature.Setup(f => f.Process(It.IsAny<RequestChain>(), It.IsAny<Request>(), It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Callback<RequestChain, Request, string, Uri, string>((c, r, v, u, p) => c.Next(r, v, u, p))
                .Returns(response);

            return requestFeature;
        }

        public static Mock<IResponseFeature> CreateResponseFeature(HttpRemoteResponse response) {
            var responseFeature = new Mock<IResponseFeature>();

            responseFeature.Setup(f => f.Process(It.IsAny<ResponseChain>(), response))
                .Callback<ResponseChain, HttpRemoteResponse>((c, r) => c.Next(r))
                .Returns(response);

            return responseFeature;
        }

        public static DynamicXmlResource GetDynamicResourceWithServiceFake(string rawUri, string contentType = "application/xml")
        {
            var uri = rawUri.StartsWith("http://") ? new Uri(rawUri) : new Uri(String.Format("file://{0}", rawUri));
            var dispatcher = new EmbeddedFileRequestDispatcher(contentType);
            var service = CreateService(rawUri, contentType);
            var response = dispatcher.Process(service.Object, "GET", uri, null);

            response.Headers.Add("X-Runtime", "29");
            response.Headers.Add("Connection", "keep-alive");
            response.Headers.Add("Cache-Control", "private, max-age=0, must-revalidate");
            response.Headers.Add("Date", "Mon, 11 Jan 2010 22:39:24 GMT");
            response.Headers.Add("ETag", "40edb82345bbb4d257708270c4cd8f76");
            response.Headers.Add("Last-Modified", "Tue, 05 Jan 2010 02:44:25 GMT");
            response.Headers.Add("Server", "nginx/0.6.39");
            response.Headers.Add("Via", "1.1 varnish");

            return new DynamicXmlResource(response, service.Object);
        }
    }
}
