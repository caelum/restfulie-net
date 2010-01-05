using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestfulieClient.resources;
using Rhino.Mocks;
using RestfulieClientTests.helpers;
using RestfulieClient.service;
using System.Net;

namespace RestfuliClientTests.helpers
{
    public class RemoteResourceFactory
    {

        public static IRemoteResourceService GetRemoteResource()
        {
            MockRepository mocks = new MockRepository();
            IRemoteResourceService remoteResourceService = mocks.DynamicMock<IRemoteResourceService>();
            HttpWebResponse response = mocks.DynamicMock<HttpWebResponse>();
            Expect.Call(remoteResourceService.Execute("", "")).IgnoreArguments().Repeat.Any().Return(new HttpRemoteResourceResponse() { XmlRepresentation = "", WebResponse = null});
            Expect.Call(remoteResourceService.GetResourceFromXml("")).IgnoreArguments().Repeat.Any().Return(new HttpRemoteResourceResponse() { XmlRepresentation = new LoadDocument().GetDocumentContent("order.xml"), WebResponse = response });
            Expect.Call(response.StatusCode).Repeat.Any().Return(HttpStatusCode.OK);
            mocks.ReplayAll();
            return remoteResourceService;
        }
    }
}
