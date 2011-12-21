using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using RestfulieClientTests.helpers;

namespace RestfulieClientTests
{
    [TestClass]
    public class DynamicXmlResourceTest
    {
        [TestMethod]
        public void ShouldBePossibleToLoadAXmlByTheyDynamicObject()
        {
            dynamic order = TestHelper.GetDynamicResourceWithServiceFake("order.xml");
            Assert.IsNotNull(order.date, "the attribute date is no expected");
            Assert.IsNotNull(order.total, "the attribute total is no expected");
        }

        [TestMethod]
        public void ShouldBePossibleToExecuteDynamicMethodsInResource()
        {
            dynamic order = TestHelper.GetDynamicResourceWithServiceFake("order.xml");
            Assert.IsNotNull(order.Pay());
        }


        [Ignore]
        public void ShouldBeAbleToAnswerToMethodRelName()
        {
            dynamic order = TestHelper.GetDynamicResourceWithServiceFake("order.xml");
            Assert.IsNotNull(order.Update());

        }

        [TestMethod]
        public void LearningToReadAAtomLinkInXml()
        {
            dynamic order = TestHelper.GetDynamicResourceWithServiceFake("order.xml");
            Assert.IsNotNull(order.Pay());
        }

        [TestMethod]
        public void ShouldBePossibleToAccessResponseHeadersEasily()
        {
            dynamic order = TestHelper.GetDynamicResourceWithServiceFake("order.xml");
            Assert.AreEqual("application/xml", order.WebResponse.Content_Type);
            Assert.AreEqual("keep-alive", order.WebResponse.Connection);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessFieldsLikeUpdateAt()
        {
            dynamic order = TestHelper.GetDynamicResourceWithServiceFake("order.xml");
            DateTime date = new DateTime(2010, 01, 01);
            Assert.AreEqual(date, order.Update_At);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessInnerFieldsInAResource()
        {
            dynamic city = TestHelper.GetDynamicResourceWithServiceFake("city.xml");
            Assert.AreEqual(18000000, city.Population.Size);
            Assert.AreEqual(10, city.Growth);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessInnerFieldsWithYourRealTypes()
        {       
            dynamic order = TestHelper.GetDynamicResourceWithServiceFake("order.xml");
            order.NumberFormatInfo = new CultureInfo("en-US", false).NumberFormat;
            Assert.AreEqual(15.00, order.Total);
            Assert.AreEqual(1, order.Id);
            Assert.AreEqual(new DateTime(2010, 01, 01), order.Update_At);
            Assert.AreEqual("unpaid", order.status);
        }

        [TestMethod]
        public void ShouldBePossibleToAccessAOtherResourceByLink()
        {
            dynamic city = TestHelper.GetDynamicResourceWithServiceFake("city.xml");
            dynamic otherCity = city.Next_Largest();
            Assert.IsNotNull(otherCity);
            Assert.AreEqual("Rio de Janeiro", otherCity.Name);
            Assert.AreEqual("Sao Paulo", city.Name);
        }
    }
}
