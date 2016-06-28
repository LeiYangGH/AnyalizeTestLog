using LogExtractor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AnyalizeTestLog
{
    /// <summary>
    /// 这个控制台程序是方便调试用的，把各种提取的内容输出到output
    /// </summary>
    public class Program
    {

        static Extractor ext;

        static void Main(string[] args)
        {
            //string inFile = Path.Combine(Environment.CurrentDirectory, @"input.txt");

            //string inFile = Path.Combine(Environment.CurrentDirectory, @"input1.txt");
            string inFile = Path.Combine(Environment.CurrentDirectory, @"large.log");
            //string outFile = Path.Combine(Environment.CurrentDirectory, @"output.txt");
            //ReadLog(inFile, outFile);
            //Process.Start(outFile);
             
            //ext = new Extractor(inFile);
            //List<Pass> lst = ext.ReadAndExtractPasses().ToList();
            //Console.WriteLine(lst[0].StartDate);
            //Console.WriteLine(lst[0].EndDate);

            Console.WriteLine("ok in main");
            Console.ReadLine();
        }

        /// <summary>
        /// 读log并输出
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outFile"></param>
        public static void ReadLog(string inFile, string outFile)
        {
            List<Pass> passes;
            using (StreamReader sr = new StreamReader(inFile, Encoding.UTF8))
            {
                string input = sr.ReadToEnd();
                passes = ext.ReadAndExtractPasses().ToList();
            }
            outFile = Path.Combine(Environment.CurrentDirectory, @"output.txt");
            using (StreamWriter sw = new StreamWriter(outFile, false, Encoding.UTF8))
            {
                foreach (var pass in passes)
                {
                    sw.Write(pass);
                }
            }
        }

    }
}


