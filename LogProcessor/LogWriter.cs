﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogProcessor
{
    public class LogWriter
    {
        private string saveFileName;
        public LogWriter(string saveFileName)
        {
            this.saveFileName = saveFileName;
        }

        /// <summary>
        /// 保存（勾选的）Pass集合。
        /// 在调用时候传进来的是已经剔除未勾选的
        /// </summary>
        /// <param name="lstPasses"></param>
        /// <returns>文件名或错误消息</returns>
        public async Task<string> SavePasses(IEnumerable<Pass> lstPasses)
        {
            try
            {
                var lines = lstPasses.Select(p => p.ToString());
                await Task.Run(() =>
                {
                    File.WriteAllText(saveFileName, string.Join("", lines));
                });
            }
            catch (Exception ex)
            {
                return string.Format(ex.Message);
            }
            return this.saveFileName;
        }
    }
}
