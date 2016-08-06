using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogProcessor
{
    public interface ILogReader
    {
        /// <summary>
        /// 按行读取log，遇到]则把之前读的一块拼接起来，并解析成Pass，最后返回Pass集合
        /// </summary>
        /// <param name="progress">进度</param>
        /// <returns>Pass集合</returns>
        Task<IList<Pass>> ReadAndExtractPasses(IProgress<ReadProgress> progress);
    }
}
