using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulieClient.service;

namespace RestfulieClientTests
{
    [TestClass]
    public class RestfulieHttpVerbDiscoveryTest
    {
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
        public void ShouldSendAPOSTIfTheStateTransitionNameIsCreate()
        {
            RestfulieHttpVerbDiscovery verbDiscovery = new RestfulieHttpVerbDiscovery();
            string verbName = verbDiscovery.GetHttpVerbByTransitionName("Create");
            Assert.AreEqual("POST", verbName);
        }

        [TestMethod]
        public void ShouldSendAPUTIfTheStateTransitionNameIsUpdate()
        {
            RestfulieHttpVerbDiscovery verbDiscovery = new RestfulieHttpVerbDiscovery();
            string verbName = verbDiscovery.GetHttpVerbByTransitionName("Update");
            Assert.AreEqual("PUT", verbName);
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
