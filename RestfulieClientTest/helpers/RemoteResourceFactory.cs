using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestfulieClient.resources;
using Rhino.Mocks;
using RestfulieClientTests.helpers;
using RestfulieClient.service;
using System.Net;

namespace RestfulieClientTests.helpers
{
    public class RemoteResourceFactory
    {

        public static IRemoteResourceService GetRemoteResource()
        {
            MockRepository mocks = new MockRepository();
            IRemoteResourceService remoteResourceService = mocks.DynamicMock<IRemoteResourceService>();
            HttpWebResponse response = mocks.DynamicMock<HttpWebResponse>();
            Expect.Call(remoteResourceService.Execute("", "")).IgnoreArguments().Repeat.Any().Return(GetHttpRemoteResponseFake());
            Expect.Call(remoteResourceService.GetResourceFromXml("")).IgnoreArguments().Repeat.Any().Return(GetHttpRemoteResponseFake() );
            Expect.Call(response.StatusCode).Repeat.Any().Return(HttpStatusCode.OK);
            mocks.ReplayAll();
            return remoteResourceService;
        }

        private static HttpRemoteResponse GetHttpRemoteResponseFake()
        {
            HttpRemoteResponse response = new HttpRemoteResponse(HttpStatusCode.OK,
                new Dictionary<string, string>(), new LoadDocument().GetDocumentContent("order.xml"));
            return response;
        }
    }
}
