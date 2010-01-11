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
            dynamic order = this.GetDynamicResourceWithServiceFake();
            Assert.IsNotNull(order.date, "the attribute date is no expected");
            Assert.IsNotNull(order.total, "the attribute total is no expected");
        }

        [TestMethod]
        public void ShouldBePossibleToExecuteDynamicMethodsInResource()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake();
            Assert.IsNotNull(order.Pay());
        }


        [Ignore]
        public void ShouldBeAbleToAnswerToMethodRelName()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake();
            Assert.IsNotNull(order.Update());

        }

        [TestMethod]
        public void LearningToReadAAtomLinkInXml()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake();
            Assert.IsNotNull(order.Pay());
        }

        [TestMethod]
        public void ShouldBePossibleToAccessResponseHeadersEasily()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake();
            Assert.AreEqual("application-xml", order.WebResponse.ContentType);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessFieldsLikeUpdateAt()
        {
            dynamic order = this.GetDynamicResourceWithServiceFake();
            Assert.AreEqual("01/01/2010", order.Update_At);
        }


        private DynamicXmlResource GetDynamicResourceWithServiceFake()
        {
            XElement element;
            IRemoteResourceService remoteService;
            string xml = new LoadDocument().GetDocumentContent("order.xml");
            element = XElement.Parse(xml);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("CONTENTTYPE", "application-xml");
            headers.Add("LASTMODIFIED", "teste");

            HttpRemoteResponse response = new HttpRemoteResponse(HttpStatusCode.OK, headers, xml);

            remoteService = this.GetRemoteServiceFake();
            return new DynamicXmlResource(response, remoteService);
        }


        private IRemoteResourceService GetRemoteServiceFake()
        {
            return RemoteResourceFactory.GetRemoteResource();
        }
    }
}
