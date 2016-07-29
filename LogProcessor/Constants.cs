using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogProcessor
{
    public static class Constants
    {
        public const int dateStringLenth = 19;
        public const int minPassSymbolDistance = 24;
        public const double fileLenth2LinesRate = 108792831d / 4150000d;
        public const string passStartString = "[";
        public const string passEndString = "]";
        public const string at = @"@";
        public const string passCharString = @"P";
        public const string failCharString = @"F";
        public const string errorCharString = @"E";
        public const int statusLastIndex = 20;
        public const string SN = @"SN";
        public const string dateFormatString = @"dd-MMM-yy  HH:mm:ss";
        public const string logExtFilter = @"log|*.log|txt|*.txt";
        public const string DateFormatString = @"[0-3]\d-(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)-1[67]  [012]\d:[0-5]\d:[0-5]\d";
        public static string strRegExtractSN = string.Format(@"{0} SN ([A-Z\d]+)\b", DateFormatString);
        public static Regex RegFindSN = new Regex(strRegExtractSN);
    }
}
