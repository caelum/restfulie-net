using RestfulieClient.service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace RestfulieClientTests
{
    [TestClass]
    public class StringValueConverterTest
    {
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
            string text = "10" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00";
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
