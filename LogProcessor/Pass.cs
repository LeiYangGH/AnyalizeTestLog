﻿using System;
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
        public List<Test> listTests = new List<Test>();
        public DateTime StartDate;//用来排序的时间

        /// <summary>
        /// 根据传进来的一段文本解析Pass
        /// </summary>
        /// <param name="testsText">包含若干Tests的那段文本</param>
        /// <param name="sdt">开始日期文本</param>
        /// <param name="edt">结束日期文本</param>
        public Pass(string testsText, string line0S, string sdt, string edt)
        {
            this.Line0S = line0S;
            this.StartDateString = sdt;
            this.EndDate = edt;
            this.StartDate = DateTime.ParseExact(this.StartDateString, Constants.dateFormatString,
    CultureInfo.InvariantCulture);
            //only for emptypass
            if (string.IsNullOrWhiteSpace(testsText))
                return;
            //Debug.Assert(this.StartDate != null && this.StartDate != new DateTime());
            //使用@拆分出各个Pass
            this.listTests = testsText.Split(new string[] { Constants.at },
                StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Length > 30).Select(x => new Test(x))
                .OrderBy(x => x.Date).ToList();
        }

        /// <summary>
        /// 点击保存时Pass的内容，可以改变return内容来测试
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this is EmptyPass)
                return (this as EmptyPass).ToString();

            //如果非空，通过Pass的日期、Tests的内容来组合成整体Pass文本
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Line0S);
            sb.Append(Constants.passStartString);
            sb.AppendLine(this.StartDateString);
            sb.Append(Constants.at);
            sb.Append(string.Join(Constants.at, this.listTests));
            sb.Append(Constants.passEndString);
            sb.Append(this.EndDate);
            sb.AppendLine();
            sb.AppendLine();
            return sb.ToString();
        }
    }

    public class EmptyPass : Pass
    {
        public EmptyPass(string testsText, string line0S, string sdt, string edt)
            : base(null, line0S, sdt, edt) { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Line0S);
            sb.Append(Constants.passStartString);
            sb.AppendLine(this.StartDateString);
            sb.Append(Constants.passEndString);
            sb.Append(this.EndDate);
            sb.AppendLine();
            return sb.ToString();
        }
    }
}