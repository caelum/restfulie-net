using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RestfulieClient.service
{
    public class StringValueConverter
    {
        private const string intPattern = "^[+-]?\\d+$";
        private const string decimalPattern = "^[+-]?\\d*separator\\d+?$";
        private const string booleanPattern = "(true|false)";
        private const string timePattern = "^((0?[1-9]|1[012])(:[0-5]\\d){0,2}(\\ [AP]M))$|^([01]\\d|2[0-3])(:[0-5]\\d){0,2}$";
        private const string datePattern = "^(((0[1-9]|[12]\\d|3[01])\\/(0[13578]|1[02])\\/((19|[2-9]\\d)\\d{2}))|((0[1-9]|[12]\\d|30)" +
             "\\/(0[13456789]|1[012])\\/((19|[2-9]\\d)\\d{2}))|((0[1-9]|1\\d|2[0-8])\\/02\\/((19|[2-9]\\d)\\d{2}))|" +
             "(29\\/02\\/((1[6-9]|[2-9]\\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$";

        private string value;
        private NumberFormatInfo numberFormatInfo;

        public bool IsDouble(string value)
        {
            return this.IsDouble(value,null);
        }

        public bool IsDouble(string value, NumberFormatInfo numberFormat = null)
        {
            string pattern = this.GetDoublePattern(numberFormat);
            return (System.Text.RegularExpressions.Regex.IsMatch(value, pattern));
        }

        public bool IsInteger(string value)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(value, intPattern));
        }

        public bool IsBoolean(string value)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(value, booleanPattern,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }

        public bool IsDateTime(string value)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(value, datePattern));
        }

        public bool IsTime(string value)
        {
            return (System.Text.RegularExpressions.Regex.IsMatch(value, timePattern));
        }

        private double ValueToDouble(string value, NumberFormatInfo numberFormat = null)
        {
            string pattern = GetDoublePattern(numberFormat);
            NumberStyles styles = NumberStyles.AllowParentheses | NumberStyles.AllowTrailingSign |
                NumberStyles.Float | NumberStyles.AllowThousands;

            return double.Parse(value, styles, numberFormat);
        }

        private string GetDoublePattern(NumberFormatInfo numberFormat = null)
        {
            if (numberFormat == null)
            {
                numberFormat = System.Globalization.NumberFormatInfo.CurrentInfo;
            }
            string escapeString = "";
            if (numberFormat.NumberDecimalSeparator.Equals(".")) escapeString = "\\";
            string pattern = Regex.Replace(decimalPattern, "separator", escapeString + numberFormat.NumberDecimalSeparator);
            return pattern;
        }

        public StringValueConverter TransformText(string text)
        {
            this.value = text;
            return this;
        }

        public StringValueConverter WithNumberFormatInfo(NumberFormatInfo nf)
        {
            this.numberFormatInfo = nf;
            return this;
        }

        public object ToValue()
        {
            if (IsInteger(this.value)) return Convert.ToInt32(this.value);
            else if (IsDouble(this.value,numberFormatInfo)) return this.ValueToDouble(this.value, numberFormatInfo);
            else if (IsBoolean(this.value)) return Convert.ToBoolean(this.value);
            else if (IsDateTime(this.value)) return Convert.ToDateTime(this.value);
            else if (IsTime(this.value)) return TimeSpan.Parse(value);
            else
                return this.value;
        }
    }
}
