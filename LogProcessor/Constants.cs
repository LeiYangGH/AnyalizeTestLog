﻿using System.Text.RegularExpressions;

namespace LogProcessor
{
    public static class Constants
    {
        public const int dateStringLenth = 19;
        public const double fileLenth2LinesRate = 108792831d / 4150000d;
        public const string passStartString = "[";
        public const string passEndString = "]";
        public const string at = @"@";
        public const string passCharString = @"P";
        public const string failCharString = @"F";
        public const string errorCharString = @"E";
        public const string SN = @"SN";
        public const string dateFormatString = @"dd-MMM-yy  HH:mm:ss";
        public const string logExtFilter = @"log|*.log|txt|*.txt";
        public const string dateRegString = @"[0-3]\d-(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)-1[67]  [012]\d:[0-5]\d:[0-5]\d";
        public static string strRegExtractStatus = string.Format(@"([""?/]){0}", dateRegString);
        public static string strRegExtractSN = string.Format(@"{0} SN ([A-Z\d]+)\b", dateRegString);
        public static Regex RegExtractSN = new Regex(strRegExtractSN, RegexOptions.Compiled);
        public static Regex RegExtractStatus = new Regex(strRegExtractStatus, RegexOptions.Compiled);
    }
}
