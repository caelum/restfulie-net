using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulieClient.features;
using RestfulieClient.request;
using RestfulieClient.service;
using RestfulieClientTests.helpers;

namespace RestfulieClientTests.features
{
    [TestClass]
    public class FollowRedirectsTests
    {
        private Mock<IRemoteResourceService> CreateService(HttpRemoteResponse response = null) {
            if (response == null)
                response = TestHelper.CreateResponse();

            var dispatcher = TestHelper.CreateDispatcher(response);
            var service = new Mock<IRemoteResourceService>();

            service.SetupGet(s => s.Dispatcher).Returns(dispatcher.Object);
            dispatcher.Setup(s => s.Process(service.Object, It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>()))
                .Returns(TestHelper.CreateResponse());

            return service;
        }

        [TestMethod]
        public void it_should_not_redirect_when_not_created_or_redirection_response() {
            var response = TestHelper.CreateResponse();
            var service = CreateService(response);
            var feature = new FollowRedirects();

            var featureResponse = feature.Process(
                new ResponseChain(service.Object, new IResponseFeature[0]), 
                response);

            Assert.AreEqual(response, featureResponse);
        }

        [TestMethod]
        public void it_should_not_redirect_when_location_header_not_present() {
            var response = TestHelper.CreateResponse(HttpStatusCode.Created);
            var service = CreateService(response);
            var feature = new FollowRedirects();

            var featureResponse = feature.Process(
                new ResponseChain(service.Object, new IResponseFeature[0]), 
                response);

            Assert.AreEqual(response, featureResponse);
        }

        [TestMethod]
        public void it_should_redirect_when_applicable() {
            var response = TestHelper.CreateResponse(HttpStatusCode.Created, new Dictionary<string, string> { { "Location", "file://NotImportant" } });
            var service = CreateService(response);
            var feature = new FollowRedirects();

            var featureResponse = feature.Process(
                new ResponseChain(service.Object, new IResponseFeature[0]), 
                response);

            Assert.AreNotEqual(response, featureResponse);
        }
    }
}
