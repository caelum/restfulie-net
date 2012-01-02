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
        [TestMethod]
        public void it_should_follow_redirects() {
            var service = new Mock<IRemoteResourceService>();
            var dispatcher = new Mock<IRequestDispatcher>();
            var chain = new ResponseChain(service.Object, new IResponseFeature[0]);
            var response = TestHelper.CreateResponse(HttpStatusCode.Created, new Dictionary<string, string> { { "Location", "file://NotImportant" } });

            service.SetupGet(s => s.Dispatcher).Returns(dispatcher.Object);

            new FollowRedirects().Process(chain, response);
            
            dispatcher.Verify(d => d.Process(service.Object, It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>()));
        }
    }
}
