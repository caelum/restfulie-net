using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulieClient.request;
using RestfulieClient.resources;
using RestfulieClient.service;

namespace RestfulieClientTests
{
    [TestClass]
    public class EntryPointServiceTests : BaseTest
    {
        private HttpRemoteResponse CreateResponse(HttpStatusCode code = HttpStatusCode.OK, Dictionary<string, string> headers = null, string content = "") {
            return new HttpRemoteResponse(code, headers ?? new Dictionary<string, string>(), content);
        }

        private IRemoteResourceService CreateService(HttpRemoteResponse response = null, IRequestDispatcher dispatcher = null) {
            if (response == null)
                response = CreateResponse();
            if (dispatcher == null)
                dispatcher = GetDispatcherFake(response).Object;

            return new EntryPointService("file://NotImportant", dispatcher);
        }

        [TestMethod]
        public void it_should_return_a_resource() {
            Assert.IsNotNull(CreateService().Get());
        }

        [TestMethod]
        public void it_should_allow_custom_request_headers() {
            var service = CreateService().With("Test", "Value");
            var headers = service.Headers;

            Assert.AreEqual(1, headers.Count);
            Assert.IsTrue(headers.ContainsKey("Test"));
            Assert.AreEqual("Value", headers["Test"], true);
        }

        [TestMethod]
        public void it_should_apply_accept_header() {
            var service = CreateService().Accepts("application/json");
            var headers = service.Headers;

            Assert.AreEqual(1, headers.Count);
            Assert.IsTrue(headers.ContainsKey("Accept"));
            Assert.AreEqual("application/json", headers["Accept"], true);
        }

        [TestMethod]
        public void it_should_apply_content_type_header() {
            var service = CreateService().As("application/json");
            var headers = service.Headers;

            Assert.AreEqual(1, headers.Count);
            Assert.IsTrue(headers.ContainsKey("Content-Type"));
            Assert.AreEqual("application/json", headers["Content-Type"], true);
        }

        [TestMethod]
        public void it_should_provide_dispatcher() {
            var dispatcher = GetDispatcherFake(It.IsAny<HttpRemoteResponse>());

            Assert.AreEqual(dispatcher.Object, CreateService(dispatcher: dispatcher.Object).Dispatcher);
        }

        [TestMethod]
        public void it_should_be_possible_to_handle_error_status() {
            var response = CreateResponse(HttpStatusCode.NotFound);

            Assert.AreEqual(HttpStatusCode.NotFound, CreateService(response).Get().WebResponse.StatusCode);
        }

        [TestMethod]
        public void it_should_return_empty_for_no_content_regardless_of_content_type() {
            var response = CreateResponse(headers: new Dictionary<string, string> { { "Content-Type", "application/xml" } });
            var resource = CreateService().Get();

            Assert.IsTrue(resource.IsEmpty);
            Assert.IsInstanceOfType(resource, typeof(EmptyResource));
        }

        [TestMethod]
        public void it_should_return_empty_for_no_content_type() {
            var response = CreateResponse(content: "{ }");
            var resource = CreateService(response).Get();

            Assert.IsTrue(resource.IsEmpty);
            Assert.IsInstanceOfType(resource, typeof(EmptyResource));
        }

        [TestMethod]
        public void it_should_use_request_feature() {
            var response = CreateResponse();
            var feature = CreateRequestFeature(response);
            var service = CreateService(response).With(feature.Object);
            
            service.Get();

            feature.Verify(f => f.Process(It.IsAny<RequestChain>(), It.IsAny<Request>(), It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void it_should_use_response_feature() {
            var response = CreateResponse();
            var feature = CreateResponseFeature(response);
            var service = CreateService(response).With(feature.Object);
            
            service.Get();

            feature.Verify(f => f.Process(It.IsAny<ResponseChain>(), It.IsAny<HttpRemoteResponse>()), Times.Once());
        }

        [TestMethod]
        public void it_should_chain_request_features() {
            var response = CreateResponse();
            var feature = CreateRequestFeature(response);
            var service = CreateService().With(feature.Object).With(feature.Object);
            
            service.Get();

            feature.Verify(f => f.Process(It.IsAny<RequestChain>(), It.IsAny<Request>(), It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void it_should_chain_response_features() {
            var response = CreateResponse();
            var feature = CreateResponseFeature(response);
            var service = CreateService(response).With(feature.Object).With(feature.Object);
            
            service.Get();

            feature.Verify(f => f.Process(It.IsAny<ResponseChain>(), It.IsAny<HttpRemoteResponse>()), Times.Exactly(2));
        }
    }
}
