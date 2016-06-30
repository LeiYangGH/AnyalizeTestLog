using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        public string DateString;
        public DateTime Date;
        public string Status;
        public string SN;
        private string testText;
        /// <summary>
        /// 通过包含Test的文本解析出Test对象
        /// </summary>
        /// <param name="testText">包含Test的文本</param>
        public Test(string testText)
        {
            this.testText = testText;
            this.DateString = this.ExtractDatetimeFormATest();
            this.Date = DateTime.ParseExact(this.DateString, Constants.dateFormatString,
                         CultureInfo.InvariantCulture);
            this.Status = this.ExtractStatusFormATest();
            this.SN = Test.ExtractSNFormATest(this.testText);
        }

        /// <summary>
        /// 提取日期
        /// </summary>
        /// <returns></returns>
        private string ExtractDatetimeFormATest()
        {
            //@26-FEB-16  14:12:16
            string dateString = this.testText.Substring(0, Constants.dateStringLenth);
            //Debug.Assert(Constants.regFindDatetime.IsMatch(dateString));
            return dateString;
        }

        /// <summary>
        /// 提取状态并转换为显示字符
        /// </summary>
        /// <returns></returns>
        private string ExtractStatusFormATest()
        {
            string trimE = this.testText.TrimEnd();
            char found = trimE[trimE.Length - 20];//Constants.dateStringLenth + 1
            switch (found)
            {
                case '\"':
                    return Constants.passCharString;
                case '?':
                    return Constants.errorCharString;
                case '/':
                    return Constants.failCharString;
                default:
                    return "";
            }
        }

        /// <summary>
        /// 提取SN, static for test
        /// </summary>
        /// <returns></returns>
        public static string ExtractSNFormATest(string input)
        {
            try
            {
                //@26-FEB-16  14:45:13 SN SS160605E
                string line0 = input.Substring(0, input.IndexOf(Environment.NewLine));
                int locSN = line0.LastIndexOf(Constants.SN);
                string sn = "";
                if (locSN > 0)
                {
                    sn = line0.Substring(locSN + 2);
                }
                return sn;
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>
        /// 点击raw或者保存时Test的内容，可以改变return内容来测试
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.testText;
        }
    }
}
