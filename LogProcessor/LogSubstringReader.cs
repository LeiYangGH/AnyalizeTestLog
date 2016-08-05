using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
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
        /// 从文本中提取Pass, 本来应该是private，public只是为了在单元测试里用
        /// </summary>
        /// <param name="passString">任意文本，在本程序中为包含（一个）[]的一段文本</param>
        /// <returns></returns>
        public Pass ExtractOnePassBySubString(string passString)
        {
            Pass p = null;

            //[在文本中的位置
            int passStartSymbolLoc = passString.IndexOf(Constants.passStartString);
            //]在文本中的位置
            int passEndSymbolLoc = passString.LastIndexOf(Constants.passEndString);
            //中间的文本（包含多个Test）
            string tests = passString.Substring(passStartSymbolLoc + 1, passEndSymbolLoc - passStartSymbolLoc - 1);
            //第一行包含S的文本
            string line0S = passString.Substring(0, passStartSymbolLoc);
            //开始时间文本
            string sdt = passString.Substring(passStartSymbolLoc + 1, Constants.dateStringLenth);
            //结束时间文本
            string edt = passString.Substring(passEndSymbolLoc + 1, Constants.dateStringLenth);
            //下面两句调试用
            //Debug.Assert(passStartSymbolLoc > 0, passStartSymbolLoc.ToString());
            //Debug.Assert(passEndSymbolLoc - passStartSymbolLoc >= Constants.dateStringLenth);
            bool hasTests = tests.Contains(Constants.at);
            p = new Pass(sdt, edt, line0S, tests);
            return p;
        }
        #endregion Extract pass-test

        /// <summary>IList<Pass>
        /// 按行读取log，遇到]则把之前读的一块拼接起来，并解析成Pass，最后返回Pass集合
        /// </summary>
        private void ReadFileToQueue(IProgress<ReadProgress> progress)
        {
            this.queue = new ConcurrentQueue<string>();
            StringBuilder sbLines4Pass = new StringBuilder();//用来暂存Pass字符串
            string passStr = null;
            string allLogString = File.ReadAllText(this.logFileName);
            foreach (string line in allLogString.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                this.readingLinesCount++;
                //根据要求保留空行
                //if (string.IsNullOrWhiteSpace(line))
                //    continue;
                sbLines4Pass.Append(line);
                if (line.Contains(Constants.passEndString))//如果这行包含了]
                {
                    passStr = sbLines4Pass.ToString();
                    if (passStr.Contains(Constants.at) && //过滤掉空Pass
                        passStr.Contains(Constants.passStartString))//log文件有不严格的[]匹配
                        queue.Enqueue(passStr);
                    sbLines4Pass.Clear();
                    if (progress != null)
                    {
                        var rprogress = new ReadProgress(this.readingLinesCount, this.logFileTotalLinesGuess);
                        progress.Report(rprogress);
                    }
                }
            }
            var rDoneprogress = new ReadProgress(this.logFileTotalLinesGuess, this.logFileTotalLinesGuess);
            progress.Report(rDoneprogress);
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


        public async Task<IList<LogProcessor.Pass>> ReadAndExtractPasses(IProgress<ReadProgress> progress)
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
