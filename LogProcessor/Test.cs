using System;
using System.Globalization;

namespace LogProcessor
{
    /// <summary>
    /// 用@分隔出来的Test
    /// </summary>
    public class Test
    {
        public string DateString;
        public DateTime Date;
        public string Status;
        public string SN;
        private string testString;
        /// <summary>
        /// 通过包含Test的文本解析出Test对象
        /// </summary>
        /// <param name="testString">包含Test的文本</param>
        public Test(string testString)
        {
            this.testString = testString;
            this.DateString = this.ExtractDatetimeFormATest();
            this.Date = DateTime.ParseExact(this.DateString, Constants.dateFormatString,
                         CultureInfo.InvariantCulture);
            this.Status = this.ExtractStatusFormATest();
            this.SN = this.ExtractSNFormATest(this.testString);
        }

        /// <summary>
        /// 提取日期
        /// </summary>
        /// <returns></returns>
        private string ExtractDatetimeFormATest()
        {
            //@26-FEB-16  14:12:16
            string dateString = this.testString.Substring(0, Constants.dateStringLenth);
            return dateString;
        }

        /// <summary>
        /// 正则提取状态并转换为显示字符，使用正则可有效避免其他位置的/符号的干扰
        /// </summary>
        /// <returns></returns>
        private string ExtractStatusFormATest()
        {
            string statusSymbolString = Constants.RegExtractStatus
                .Match(this.testString).Groups[1].Value;
            switch (statusSymbolString)
            {
                case @"""":
                    return Constants.passCharString;
                case @"?":
                    return Constants.errorCharString;
                case @"/":
                    return Constants.failCharString;
                default:
                    return "";
            }
        }

        /// <summary>
        /// 正则提取SN
        /// </summary>
        /// <returns></returns>
        public string ExtractSNFormATest(string input)
        {
            //@26-FEB-16  14:45:13 SN SS160605E
            string sn = Constants.RegExtractSN.Match(input).Groups[1].Value;
            return sn;
        }

        /// <summary>
        /// 点击raw或者保存时Test的内容，可以改变return内容来测试
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.testString;
        }
    }
}
