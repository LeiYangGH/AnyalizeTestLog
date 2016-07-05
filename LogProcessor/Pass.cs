using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogProcessor
{
    /// <summary>
    /// []直接的内容，包含多个Test
    /// </summary>
    public class Pass
    {
        protected string Line0S;
        public string StartDateString;
        public string EndDate;
        public readonly bool HasTests;
        public List<Test> listTests = new List<Test>();
        public DateTime StartDate;//用来排序的时间

        /// <summary>
        /// 只在保存时候用
        /// </summary>
        private Pass(bool hasTests)
        {
            this.HasTests = hasTests;
        }

        /// <summary>
        /// 根据传进来的一段文本解析Pass
        /// </summary>
        /// <param name="testsText">包含若干Tests的那段文本</param>
        /// <param name="sdt">开始日期文本</param>
        /// <param name="edt">结束日期文本</param>
        public Pass(string sdt, string edt, string line0S, bool hasTests, string testsText)
        {
            this.Line0S = line0S;
            this.StartDateString = sdt;
            this.EndDate = edt;
            this.StartDate = DateTime.ParseExact(this.StartDateString,
                Constants.dateFormatString, CultureInfo.InvariantCulture);
            this.HasTests = hasTests;
            if (this.HasTests)
            {
                //Debug.Assert(this.StartDate != null && this.StartDate != new DateTime());
                //使用@拆分出各个Pass
                this.listTests = testsText.Split(new string[] { Constants.at },
                    StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.Length > 30).Select(x => new Test(x))
                    //.OrderBy(x => x.Date)//不改变写时顺序
                    .ToList();
            }
        }


        private Pass clone = null;
        /// <summary>
        /// 新的Pass供保存用
        /// </summary>
        public Pass PassClonedWithBasicProperties
        {
            get
            {
                if (this.clone == null)
                {
                    this.clone = new Pass(this.HasTests);
                    clone.Line0S = this.Line0S;
                    clone.StartDateString = this.StartDateString;
                    clone.EndDate = this.EndDate;
                }
                return this.clone;
            }
        }

        /// <summary>
        /// 点击保存时Pass的内容，
        /// 通过日期、Tests等参数来组合
        /// 可以改变return内容来测试
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Line0S);
            sb.Append(Constants.passStartString);
            sb.AppendLine(this.StartDateString);
            if (this.HasTests)
            {
                sb.Append(Constants.at);
                sb.Append(string.Join(Constants.at, this.listTests));
            }
            sb.Append(Constants.passEndString);
            sb.Append(this.EndDate);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
