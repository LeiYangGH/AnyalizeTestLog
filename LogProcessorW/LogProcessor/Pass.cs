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
    /// []直接的内容，包含多个Test
    /// </summary>
    public class Pass
    {
        public string StartDateString;
        public string EndDate;
        public IList<Test> ListTests = new List<Test>();
        public DateTime StartDate;//用来排序的时间

        public Pass(string passText, string sdt, string edt)
        {
            this.StartDateString = sdt;
            this.EndDate = edt;

            //only for emptypass
            if (string.IsNullOrWhiteSpace(passText))
                return;

            this.StartDate = DateTime.ParseExact(this.StartDateString, Constants.dateFormatString,
                CultureInfo.InvariantCulture);
            Debug.Assert(this.StartDate != null && this.StartDate != new DateTime());
            //使用@拆分
            foreach (string test in passText.Split(new string[] { Constants.at },
                StringSplitOptions.RemoveEmptyEntries))
            {
                if (test.Length < 30)//去掉拆分出来的垃圾段
                    continue;
                Test t = new Test(test);
                this.ListTests.Add(t);
            }
        }

        //调试用
        public override string ToString()
        {
            if (this is EmptyPass)
                return (this as EmptyPass).ToString();

            if (this.ListTests.Count == 0)
            {
                return string.Format("[{0}]{1}", this.StartDateString, this.EndDate);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(Constants.passStartString);
            sb.AppendLine(this.StartDateString);
            sb.Append(Constants.at);
            sb.Append(string.Join(Constants.at, this.ListTests));
            sb.Append(Constants.passEndString);
            sb.Append(this.EndDate);
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }
    }

    public class EmptyPass : Pass
    {
        private string passEmptyText;

        public EmptyPass(string passText, string sdt, string edt)
            : base(null, sdt, edt)
        {
            this.passEmptyText = passText;
        }

        public override string ToString()
        {
            return this.passEmptyText;
        }
    }
}
