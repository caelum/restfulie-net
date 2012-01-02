using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulieClient.features;
using RestfulieClient.request;
using RestfulieClient.service;
using RestfulieClientTests.helpers;

namespace RestfulieClientTests.features
{
    [TestClass]
    public class AutoRefreshTests
    {
        [TestMethod]
        public void it_should_auto_refresh() {
            var service = new Mock<IRemoteResourceService>();
            var dispatcher = new Mock<IRequestDispatcher>();
            var chain = new ResponseChain(service.Object, new IResponseFeature[0]);
            var response = TestHelper.CreateResponse(headers: new Dictionary<string, string> { { "Refresh", "5; url=http://NotImportant" } });

            service.SetupGet(s => s.IsAsynch).Returns(true);
            service.SetupGet(s => s.Dispatcher).Returns(dispatcher.Object);

            new AutoRefresh().Process(chain, response);

            dispatcher.Verify(d => d.Process(service.Object, It.IsAny<string>(), It.IsAny<Uri>(), It.IsAny<string>()));
        }
    }
}
