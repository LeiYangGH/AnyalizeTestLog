using LogProcessor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor
{
    /// <summary>
    /// 使用Substring来解析，特点是速度快，缺点是灵活性差，要求Log文件规范
    /// 使用队列来存储每个Pass文本，提取与读文件同时进行提高速度
    /// </summary>
    public class LogSubstringReader : ILogReader
    {
        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private string logFileName;
        private long logFileTotalLinesGuess;
        private long readingLinesCount;
        private bool readCompleted = false;

        public LogSubstringReader(string logFileName)
        {
            this.logFileName = logFileName;
        }

        #region Extract pass-test

        /// <summary>
        /// 从文本中提取Pass
        /// </summary>
        /// <param name="input">任意文本，在本程序中为包含（一个）[]的一段文本</param>
        /// <returns></returns>
        private Pass ExtractOnePassBySubString(string input)
        {
            Pass p = null;

            //[在文本中的位置
            int passStartSymbolLoc = input.IndexOf(Constants.passStartString);
            //]在文本中的位置
            int passEndSymbolLoc = input.LastIndexOf(Constants.passEndString);
            //中间的文本（包含多个Test）
            string tests = input.Substring(passStartSymbolLoc + 1, passEndSymbolLoc - passStartSymbolLoc - 1);
            //第一行包含S的文本
            string line0S = input.Substring(0, passStartSymbolLoc);
            //开始时间文本
            string sdt = input.Substring(passStartSymbolLoc + 1, Constants.dateStringLenth);
            //结束时间文本
            string edt = input.Substring(passEndSymbolLoc + 1, Constants.dateStringLenth);
            //下面两句调试用
            //Debug.Assert(passStartSymbolLoc > 0, passStartSymbolLoc.ToString());
            //Debug.Assert(passEndSymbolLoc - passStartSymbolLoc >= Constants.dateStringLenth);
            if (passEndSymbolLoc - passStartSymbolLoc > Constants.minPassSymbolDistance)
                p = new Pass(tests, line0S, sdt, edt);
            else//[]之间没有@的空Pass
                p = new EmptyPass(tests, line0S, sdt, edt);
            return p;

        }
        #endregion Extract pass-test


        /// <summary>IList<Pass>
        /// 按行读取log，遇到]则把之前读的一块拼接起来，并解析成Pass，最后返回Pass集合
        /// </summary>
        private void ReadFileToQueue(System.Threading.IProgress<ReadProgress> progress)
        {
            this.queue = new ConcurrentQueue<string>();
            StringBuilder sbLines4Pass = new StringBuilder();//用来暂存Pass字符串
            string passStr = null;
            foreach (string line in File.ReadLines(logFileName, Encoding.UTF8))
            {
                this.readingLinesCount++;
                //根据要求保留空行
                //if (string.IsNullOrWhiteSpace(line))
                //    continue;
                sbLines4Pass.AppendLine(line);
                if (line.Contains(Constants.passEndString))//如果这行包含了]
                {
                    passStr = sbLines4Pass.ToString();
                    if (passStr.Contains(Constants.passStartString))//log文件有不严格的[]匹配
                        queue.Enqueue(passStr);
                    sbLines4Pass.Clear();
                    if (progress != null)
                    {
                        var rprogress = new ReadProgress(this.readingLinesCount, this.logFileTotalLinesGuess);
                        progress.Report(rprogress);
                    }
                }
            }
            this.readCompleted = true;
        }

        /// <summary>
        /// 不断从队列取出Pass文本，并提取出Pass对象
        /// </summary>
        /// <returns></returns>
        private IList<Pass> ExtractPassesFromQueue()
        {
            List<Pass> listPasses = new List<Pass>();
            string passStr;

            while (!this.readCompleted)
            {
                if (this.queue.TryDequeue(out passStr))
                {
                    Pass pass = this.ExtractOnePassBySubString(passStr);
                    listPasses.Add(pass);
                }
            }

            while (!this.queue.IsEmpty)
            {
                if (this.queue.TryDequeue(out passStr))
                {
                    Pass pass = this.ExtractOnePassBySubString(passStr);
                    listPasses.Add(pass);
                }
            }
            return listPasses;
        }

        #region private methords
        /// 估算文件行数

        private long GuessLogFileLineCount()
        {
            long len = new FileInfo(logFileName).Length;
            return (long)(len / Constants.fileLenth2LinesRate);
        }

        #endregion private methords


        public async Task<IList<global::LogProcessor.Pass>> ReadAndExtractPasses(System.Threading.IProgress<ReadProgress> progress)
        {
            this.logFileTotalLinesGuess = this.GuessLogFileLineCount();

            this.readingLinesCount = 0;

            IList<Pass> lstPasses = null;

            await Task.Run(() =>
            {
                this.readCompleted = false;//重要
                var read = Task.Run(() =>
                {
                    ReadFileToQueue(progress);
                });
                var extract = Task.Run(() =>
                {
                    lstPasses = ExtractPassesFromQueue();
                });
                Task.WaitAll(new Task[] { read, extract });
            });
            return lstPasses;
        }
    }
}
