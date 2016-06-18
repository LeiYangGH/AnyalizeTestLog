using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LogProcessor
{
    /// <summary>
    /// 用@分隔出来的Test
    /// </summary>
    public class Test
    {
        public const string passSymbol = @"""";
        public const string failSymbol = @"/";
        public const string errorSymbol = @"?";
        public string Date;
        public string Status;
        public string SN;
        private string testText;
        public Test(string testText)
        {
            this.testText = testText;
            this.Date = this.ExtractDatetimeFormATest();
            this.Status = this.ExtractStatusFormATest();
            this.SN = ExtractSNFormATest(this.testText);
        }

        /// <summary>
        /// 提取日期
        /// </summary>
        /// <returns></returns>
        private string ExtractDatetimeFormATest()
        {
            //@26-FEB-16  14:12:16
            string date = Constants.RegFindDatetime.Match(this.testText).Groups[0].Value;
            return date;
        }

        /// <summary>
        /// 提取状态并转换为显示字符
        /// </summary>
        /// <returns></returns>
        private string ExtractStatusFormATest()
        {
            if (this.testText.Contains(passSymbol))
                return "P";
            else if (this.testText.Contains(errorSymbol))
                return "E";
            else if (this.testText.Contains(failSymbol))
                return "F";
            else
                return "";
        }

        /// <summary>
        /// 提取SN
        /// </summary>
        /// <returns></returns>
        public static string ExtractSNFormATest(string input)
        {
            //@26-FEB-16  14:45:13 SN SS160605E
            string sn = Constants.RegFindSN.Match(input).Groups[1].Value;
            return sn;
        }

        /// <summary>
        /// 调试用
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.testText;
        }
    }
}
