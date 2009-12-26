using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestfulieClient.resources;
using Rhino.Mocks;
using RestfulieClientTests.helpers;

namespace RestfuliClientTests.helpers
{
    public class RemoteResourceFactory
    {

        public static IRemoteResourceService GetRemoteResource()
        {
            MockRepository mocks = new MockRepository();
            IRemoteResourceService remoteResourceService = mocks.DynamicMock<IRemoteResourceService>();
            Expect.Call(remoteResourceService.Execute("", "")).IgnoreArguments().Repeat.Any().Return(new Object());
            Expect.Call(remoteResourceService.GetResourceFromXml("")).IgnoreArguments().Repeat.Any().Return(new LoadDocument().GetDocumentContent("order.xml"));
            mocks.ReplayAll();
            return remoteResourceService;
        }
    }
}
