using RestfulieClient.service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace RestfulieClientTests
{
    /// <summary>
    ///This is a test class for StringValueConverterTest and is intended
    ///to contain all StringValueConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringValueConverterTest
    {
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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void ShouldBePossibleToVerifyStringHasBooleanValue()
        {
            StringValueConverter converter = new StringValueConverter();
            Assert.IsTrue(converter.IsBoolean("true"));
            Assert.IsTrue(converter.IsBoolean("false"));
        }

        [TestMethod]
        public void ShouldBePossibleToVerifyStringHasDoubleValue()
        {
            StringValueConverter converter = new StringValueConverter();
            string text = "10" + System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00";
            Assert.IsTrue(converter.IsDouble(text));
        }

        [TestMethod]
        public void ShouldBePossibleToVerifyStringHasDoubleValueSettingNumberFormatInfo()
        {
            StringValueConverter converter = new StringValueConverter();
            string text = "11.85";
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            Assert.IsTrue(converter.IsDouble(text, nfi));
        }

        [TestMethod]
        public void ShouldBePossibleToVerifyStringHasIntegerValue()
        {
            StringValueConverter converter = new StringValueConverter();
            string text = "11";
            Assert.IsTrue(converter.IsInteger(text));
        }

        [TestMethod]
        public void ShouldBePossibleToVerifyStringHasTimeValue()
        {
            StringValueConverter converter = new StringValueConverter();
            string text = "11:45";
            Assert.IsTrue(converter.IsTime(text));
        }

        [TestMethod]
        public void ShouldBePossibleToVerifyStringHasDateTimeValue()
        {
            StringValueConverter converter = new StringValueConverter();
            string text = "10/01/2010";
            Assert.IsTrue(converter.IsDateTime(text));
        }

        [TestMethod]
        public void ShouldBePossibleToConvertStringToDate()
        {
            StringValueConverter converter = new StringValueConverter();
            DateTime value = (DateTime)converter.TransformText("11/01/2010").ToValue();
            Assert.AreEqual(new DateTime(2010, 01, 11), value);
        }

        [TestMethod]
        public void ShouldBePossibleToConvertStringToTime()
        {
            StringValueConverter converter = new StringValueConverter();
            TimeSpan value = (TimeSpan)converter.TransformText("09:45").ToValue();
            Assert.AreEqual(new TimeSpan(9, 45, 0), value);
        }

    }
}
