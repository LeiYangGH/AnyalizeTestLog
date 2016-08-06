
namespace LogProcessor
{
    public struct ReadProgress
    {
        public ReadProgress(long readingLinesCount, long logFileTotalLinesGuess)
        {
            this.ReadingLinesCount = readingLinesCount;
            if (ReadingLinesCount >= logFileTotalLinesGuess)
                this.ReadingPercent = 100;
            else
                this.ReadingPercent = (int)(ReadingLinesCount / (double)logFileTotalLinesGuess * 100d);
            this.Message = string.Format("{0} lines read", ReadingLinesCount);
        }

        public long ReadingLinesCount;
        public int ReadingPercent;
        public string Message;
    }
}
