using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulieClient.resources;

namespace RestfulieClientTests
{
    [TestClass]
    public class EntryPointTests : BaseTest
    {
        [TestMethod]
        public void ShouldBePossibleToGetAResourceRepresentationByTheEntryFluentInteface()
        {
            dynamic order = Restfulie.At(GetServiceFake("order.xml")).Get();
            Assert.IsNotNull(order);
            Assert.IsNotNull(order.date, "the attribute date is no expected");
            Assert.IsNotNull(order.total, "the attribute total is no expected");
        }

        [TestMethod]
        public void ShouldBePossibleDefineConfigurationOfEntryPointService()
        {
            dynamic entryPointService = Restfulie.At(GetServiceFake("http:\\localhost:3000\\order\\1.xml"));
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
            dynamic entryPointService = Restfulie.At(GetServiceFake(""));
            dynamic order = entryPointService.Get();
        }

        [TestMethod]
        public void ShoudBePossibleToGetAWebReponse()
        {
            dynamic order = Restfulie.At(GetServiceFake("http:\\localhost:3000\\order\\1.xml")).Get();
            Assert.IsNotNull(order);
            Assert.IsNotNull(order.WebResponse);
        }

        [TestMethod]
        public void ShouldBePossibleToGetTheResponseStatusCode()
        {
            dynamic order = Restfulie.At(GetServiceFake("http:\\localhost:3000\\order\\1.xml")).Get();
            Assert.IsNotNull(order.WebResponse.StatusCode);
        }

        [TestMethod]
        public void ShouldBePossibleToCreateResourcesFromEntryPoint()
        {
            string resourceXml ="<city> " +
                                " <name>Minas Gerais</name> " +
                                " <population> " +
                                "   <size>230000</size> " +
                                "   <growth>15</growth> " +
                                " </population> " +
                                " <updated-at>10/01/2010</updated-at> " +
                                " <link rel=\"next_largest\" href=\"city.xml\" /> " +
                                "</city> ";
            dynamic newCity = Restfulie.At(GetServiceFake("http://localhost:3000/cities")).Create(resourceXml);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, newCity.WebResponse.StatusCode);
        }
    }
}
