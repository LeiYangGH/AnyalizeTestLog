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
        public const double fileLenth2LinesRate = 108792831d/4224929d;
        public const string passStartString = "[";
        public const string passEndString = "]";
        public const string At = @"@";
        public const string DatePatternString = @"[0-3]\d-(?:JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)-1[67]  [012]\d:[0-5]\d:[0-5]\d";

        public static string strRegFindPasses = string.Format(@"\[({0})([\s\S]*)]({0})", DatePatternString);
        public static string strRegExtractSN = string.Format(@"{0} SN ([A-Z\d]+)\b", DatePatternString);
      
        public static Regex regFindPasses = new Regex(strRegFindPasses, RegexOptions.Compiled);
        public static Regex RegFindDatetime = new Regex(DatePatternString, RegexOptions.Compiled);
        public static Regex RegFindSN = new Regex(strRegExtractSN, RegexOptions.Compiled);
    }
}
