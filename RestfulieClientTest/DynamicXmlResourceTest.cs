using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulieClientTests.helpers;
using System.Xml.Linq;
using Rhino.Mocks;
using RestfulieClient.resources;
using RestfulieClient.service;
using System.Net;

namespace RestfulieClientTests
{
    /// <summary>
    /// Summary description for DynamicXmlResourceTest
    /// </summary>
    [TestClass]
    public class DynamicXmlResourceTest
    {
        public DynamicXmlResourceTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        [TestMethod]
        public void ShouldBePossibleToLoadAXmlByTheyDynamicObject()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake("order.xml");
            Assert.IsNotNull(order.date, "the attribute date is no expected");
            Assert.IsNotNull(order.total, "the attribute total is no expected");
        }

        [TestMethod]
        public void ShouldBePossibleToExecuteDynamicMethodsInResource()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake("order.xml");
            Assert.IsNotNull(order.Pay());
        }


        [Ignore]
        public void ShouldBeAbleToAnswerToMethodRelName()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake("order.xml");
            Assert.IsNotNull(order.Update());

        }

        [TestMethod]
        public void LearningToReadAAtomLinkInXml()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake("order.xml");
            Assert.IsNotNull(order.Pay());
        }

        [TestMethod]
        public void ShouldBePossibleToAccessResponseHeadersEasily()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake("order.xml");
            Assert.AreEqual("application/xml", order.WebResponse.ContentType);
            Assert.AreEqual("keep-alive", order.WebResponse.Connection);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessFieldsLikeUpdateAt()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake("order.xml");
            Assert.AreEqual("01/01/2010", order.Update_At);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessInnerFieldsInAResource()
        {
            dynamic city = this.GetDynamicResourceWithServiceFake("city.xml");
            Assert.AreEqual("18000000", city.Population.Size);
            Assert.AreEqual("10", city.Growth);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessAOtherResourceByLink()
        {
            dynamic city = this.GetDynamicResourceWithServiceFake("city.xml");
            dynamic otherCity = city.Next_Largest();
            Assert.IsNotNull(otherCity);
            Assert.AreEqual("Sao Paulo", otherCity.Name);
            
        }


        private DynamicXmlResource GetDynamicResourceWithServiceFake(string fileName)
        {
            XElement element;
            IRemoteResourceService remoteService;
            string xml = new LoadDocument().GetDocumentContent(fileName);
            element = XElement.Parse(xml);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("XRUNTIME", "29");
            headers.Add("CONNECTION", "keep-alive");
            headers.Add("CONTENTTYPE", "application/xml");
            headers.Add("CACHECONTROL", "private, max-age=0, must-revalidate");
            headers.Add("DATE", "Mon, 11 Jan 2010 22:39:24 GMT");
            headers.Add("ETAG", "40edb82345bbb4d257708270c4cd8f76");
            headers.Add("LASTMODIFIED", "Tue, 05 Jan 2010 02:44:25 GMT");
            headers.Add("SERVER", "nginx/0.6.39");
            headers.Add("VIA", "1.1 varnish");
            HttpRemoteResponse response = new HttpRemoteResponse(HttpStatusCode.OK, headers, xml);

            remoteService = this.GetRemoteServiceFake(fileName);
            return new DynamicXmlResource(response, remoteService);
        }


        private IRemoteResourceService GetRemoteServiceFake(string fileName)
        {
            return RemoteResourceFactory.GetRemoteResource(fileName);
        }
    }
}
