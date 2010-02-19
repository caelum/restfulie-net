using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulieClient.resources;
using RestfulieClientTests.helpers;

namespace RestfulieClientTests
{
    /// <summary>
    /// Summary description for EntryPointTests
    /// </summary>
    [TestClass]
    public class EntryPointTests
    {
        public EntryPointTests()
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
        public void ShouldBePossibleToGetAResourceRepresentationByTheEntryFluentInteface()
        {
            dynamic order = Restfulie.At(GetEntryPointServiceForTests("order.xml")).Get();
            Assert.IsNotNull(order);
            Assert.IsNotNull(order.date, "the attribute date is no expected");
            Assert.IsNotNull(order.total, "the attribute total is no expected");
        }

        [TestMethod]
        public void ShouldBePossibleDefineConfigurationOfEntryPointService()
        {
            dynamic entryPointService = Restfulie.At(GetEntryPointServiceForTests("order.xml"));
            Assert.IsNotNull(entryPointService);
        }

        [TestMethod]
        public void ShouldHasAnInstanceOfDefaultEntryPointServiceDefinedWithoutSetAConfiguration()
        {
           dynamic entryPointService = Restfulie.At("uri");
            Assert.IsNotNull(entryPointService);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldBeThrowAnErrorIfTheInvokeGetMethodWithoutUriDefined()
        {
            IRemoteResourceService entryPointService = Restfulie.At(GetEntryPointServiceForTests(""));
            dynamic order = entryPointService.Get();
        }

        [TestMethod]
        public void ShoudBePossibleToGetAWebReponse()
        {
            IRemoteResourceService serviceMock = GetEntryPointServiceForTests("http:\\localhost:3000\\order\\1.xml");
            dynamic order = Restfulie.At(serviceMock).Get();
            Assert.IsNotNull(order.WebResponse);
        }

        [TestMethod]
        public void ShouldBePossibleToGetTheResponseStatusCode()
        {
            dynamic order = Restfulie.At(GetEntryPointServiceForTests("http:\\localhost:3000\\order\\1.xml")).Get();
            Assert.IsNotNull(order.WebResponse.StatusCode);
        }

        [TestMethod]
        public void ShouldBePossibleToPostSomeContent()
        {
            string content = "<order><date>19/02/2010</date><total>55.00</total></order>";
            dynamic newOrder = Restfulie.At(GetEntryPointServiceForTests("http:\\localhost:3000\\orders")).Create(content);
            Assert.IsNotNull(newOrder);
            Assert.AreEqual("55.00",newOrder.total);
        }

        private IRemoteResourceService GetEntryPointServiceForTests(string uri)
        {
            return RemoteResourceFactory.GetRemoteResource(uri);
        }
    }
}
