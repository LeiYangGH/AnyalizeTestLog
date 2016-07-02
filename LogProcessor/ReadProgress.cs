using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor
{
    public struct ReadProgress
    {
        public ReadProgress(long readingLinesCount, long logFileTotalLinesGuess)
        {
            this.ReadingLinesCount = readingLinesCount;
            if (ReadingLinesCount > logFileTotalLinesGuess)
                ReadingLinesCount = logFileTotalLinesGuess;

            this.ReadingPercent = (int)(ReadingLinesCount / (double)logFileTotalLinesGuess * 100d);
            this.Message = string.Format("{0} lines read", ReadingLinesCount);
        }

        public long ReadingLinesCount;
        public int ReadingPercent;
        public string Message;
    }
}
