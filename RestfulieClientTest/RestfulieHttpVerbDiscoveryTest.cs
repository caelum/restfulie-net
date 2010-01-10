using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulieClient.service;

namespace RestfulieClientTests
{
    /// <summary>
    /// Summary description for RestfulieHttpVerbDiscoveryTest
    /// </summary>
    [TestClass]
    public class RestfulieHttpVerbDiscoveryTest
    {
        public RestfulieHttpVerbDiscoveryTest()
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
        public void ShouldSendADELETEIfTheStateTransitionNameIsCancelDestroyOrDelete()
        {
            RestfulieHttpVerbDiscovery verbDiscovery = new RestfulieHttpVerbDiscovery();
            string verbName = verbDiscovery.GetHttpVerbByTransitionName("Cancel");
            List<string> transitionNames = new List<string>() { "Cancel", "Destroy", "Delete" };
            transitionNames.ForEach(transitionName =>
                    Assert.AreEqual("DELETE", verbDiscovery.GetHttpVerbByTransitionName(transitionName)) 
            );            
            
        }
        [TestMethod]
        public void ShouldSendAPOSTIfTheStateTransitionNameIsUpdate()
        {
            RestfulieHttpVerbDiscovery verbDiscovery = new RestfulieHttpVerbDiscovery();
            string verbName = verbDiscovery.GetHttpVerbByTransitionName("Update");
            Assert.AreEqual("POST", verbName);
        }

        [TestMethod]
        public void ShouldBeSendAGetIfTheStateTransitionNameIsRefreshReloadShowOrLatest()
        {  
            RestfulieHttpVerbDiscovery verbDiscovery = new RestfulieHttpVerbDiscovery();            
            List<string> transitionNames = new List<string>() { "Refresh", "Reload", "Show", "Latest" };
            transitionNames.ForEach(transitionName => 
                    Assert.AreEqual("GET",verbDiscovery.GetHttpVerbByTransitionName(transitionName)
            ));
              
        }        

        [TestMethod]
        public void ShouldBeSendAGetForSomeTransitionNameThatIsNotSupported()
        {
            RestfulieHttpVerbDiscovery verbDiscovery = new RestfulieHttpVerbDiscovery();
            string verbName = verbDiscovery.GetHttpVerbByTransitionName("Transition Name not default");
            Assert.AreEqual("GET", verbName);
        }
    }
}
