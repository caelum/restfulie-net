using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulieClientTests.helpers;

namespace RestfulieClientTests
{
    [TestClass]
    public class DynamicJsonResourceTest
    {
        [TestMethod]
        public void ShouldBePossibleToLoadAJsonByTheDynamicObject()
        {
            dynamic order = TestHelper.GetDynamicJsonResourceWithServiceFake("order.json");
            Assert.IsNotNull(order.date, "the attribute date is no expected");
            Assert.IsNotNull(order.total, "the attribute total is no expected");
        }

        [TestMethod]
        public void ShouldBePossibleToExecuteDynamicMethodsInResource()
        {
            dynamic order = TestHelper.GetDynamicJsonResourceWithServiceFake("order.json");
            Assert.IsNotNull(order.Pay());
        }


        [Ignore]
        public void ShouldBeAbleToAnswerToMethodRelName()
        {
            dynamic order = TestHelper.GetDynamicJsonResourceWithServiceFake("order.json");
            Assert.IsNotNull(order.Update());

        }

        [TestMethod]
        public void LearningToReadAAtomLinkInXml()
        {
            dynamic order = TestHelper.GetDynamicJsonResourceWithServiceFake("order.json");
            Assert.IsNotNull(order.Pay());
        }

        [TestMethod]
        public void ShouldBePossibleToAccessResponseHeadersEasily()
        {
            dynamic order = TestHelper.GetDynamicJsonResourceWithServiceFake("order.json");
            Assert.AreEqual("application/json", order.WebResponse.Content_Type);
            Assert.AreEqual("keep-alive", order.WebResponse.Connection);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessFieldsLikeUpdateAt()
        {
            dynamic order = TestHelper.GetDynamicJsonResourceWithServiceFake("order.json");
            DateTime date = new DateTime(2010, 01, 01);
            Assert.AreEqual(date, order.Update_At);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessInnerFieldsInAResource()
        {
            dynamic city = TestHelper.GetDynamicJsonResourceWithServiceFake("city.json");
            Assert.AreEqual(18000000, city.Population.Size);
            Assert.AreEqual(10, city.Population.Growth);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessInnerFieldsWithYourRealTypes()
        {       
            dynamic order = TestHelper.GetDynamicJsonResourceWithServiceFake("order.json");
            order.NumberFormatInfo = new CultureInfo("en-US", false).NumberFormat;
            Assert.AreEqual(15.00, order.Total);
            Assert.AreEqual(1, order.Id);
            Assert.AreEqual(new DateTime(2010, 01, 01), order.Update_At);
            Assert.AreEqual("unpaid", order.status);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessAnotherResourceByLink()
        {
            dynamic city = TestHelper.GetDynamicJsonResourceWithServiceFake("city.json");
            dynamic otherCity = city.Next_Largest();
            Assert.IsNotNull(otherCity);
            Assert.AreEqual("Rio de Janeiro", otherCity.Name);
            Assert.AreEqual("Sao Paulo", city.Name);
        }
    }
}
