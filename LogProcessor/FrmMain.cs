using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
namespace LogProcessor
{
    public partial class FrmMain : Form
    {
        #region Fields

        //默认的log文件
        private static string curDir = Environment.CurrentDirectory;
        //private static string logFileName = Path.Combine(curDir, @"input1.txt");
        //private static string logFileName = Path.Combine(curDir, @"inputedit.txt");
        //private static string logFileName = Path.Combine(curDir, @"inputemptypass.txt");
        private static string logFileName = Path.Combine(Environment.CurrentDirectory, @"large.log");
        private const double bit2M = 1024 * 1024;
        private long logFileTotalLinesGuess;
        private int readingLinesCount;
        private List<Pass> listPasses;
        #endregion Fields

        //test
        private void button1_Click(object sender, EventArgs e)
        {
            long cnt = this.GuessLogFileLineCount();
            long l = cnt;
        }

        public FrmMain()
        {
            InitializeComponent();
            ResetColumnOrders();
        }

        #region Properties

        #endregion Properties

        private async Task<long> Go()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            this.InitValues();

            var readProgress = new EventProgress<long>();
            readProgress.ProgressChanged += (s, e) =>
            {
                this.UpdateReadProgress(e.Value);
            };

            var rePasses = await this.ReadLineByLine(readProgress);
            this.listPasses = rePasses.ToList();

            if (this.chkSort.Checked)
                this.listPasses = this.listPasses.OrderBy(x => x.StartDate).ToList();

            if (this.listPasses.Count > 0)
            {
                this.DisplayPasses(this.listPasses);
                this.btnSave.Enabled = true;
            }

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }


        #region Read log file
        /// <summary>
        /// 按行读取log，遇到]则把之前读的一块拼接起来放入队列
        /// </summary>
        private async Task<IList<Pass>> ReadLineByLine(System.Threading.IProgress<long> progress)
        {
            List<Pass> lst = new List<Pass>();
            StringBuilder sbLine = new StringBuilder();
            foreach (string line in File.ReadLines(logFileName))
            {
                this.readingLinesCount++;
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                sbLine.AppendLine(line);
                if (line.Contains(Constants.passEndString))
                {
                    IList<Pass> passes = await ExtractPassesFromInputByRegex(sbLine.ToString());
                    lst.AddRange(passes);
                    //if (passStrings.Count >= 50) break;//for debugging
                    sbLine.Clear();
                    if (progress != null)
                        progress.Report(this.readingLinesCount);
                }
            }
            return lst;
        }
        #endregion Read log file


        #region Form UI


        private async void btnExtractTests_Click(object sender, EventArgs e)
        {
            this.btnExtractTests.Enabled = false;
            long sec = await this.Go();
            this.btnExtractTests.Enabled = true;
            this.statusLblPerformance.Text = string.Format("Time used（ms）： {0}", sec);
        }

        //更新读文件进度
        private void UpdateReadProgress(long reading)
        {
            if (reading > this.logFileTotalLinesGuess)
                reading = logFileTotalLinesGuess;
            this.progressBar.Value = (int)(reading / (double)this.logFileTotalLinesGuess * 100d);
            this.statusLblMsg.Text = string.Format("{0} lines read", reading);
        }

        /// <summary>
        /// 显示所有Pass
        /// </summary>
        /// <param name="lstP"></param>
        private void DisplayPasses(List<Pass> lstP)
        {
            this.lvwTests.ItemChecked -= this.lvwTests_ItemChecked;
            this.lvwTests.BeginUpdate();
            this.lvwTests.Items.Clear();

            foreach (var p in this.listPasses) //按日期排序
            {
                //开始符号
                ListViewItem4Log item0 = new ListViewItem4Log(p, true);
                ListViewItem4Log itemz = new ListViewItem4Log(p, false);

                IList<ListViewItem4Log> passes = new List<ListViewItem4Log>() 
                {
                    item0,itemz
                };

                item0.ParentPasses = passes;
                itemz.ParentPasses = passes;

                this.lvwTests.Items.Add(item0);

                IList<ListViewItem4Log> children = new List<ListViewItem4Log>();
                //每个Test
                foreach (var t in p.ListTests)
                {
                    ListViewItem4Log item = new ListViewItem4Log(p, t);
                    item.ParentPasses = passes;
                    this.lvwTests.Items.Add(item);
                    children.Add(item);
                }

                this.lvwTests.Items.Add(itemz);

                item0.ChildrenTestItems = children;
                itemz.ChildrenTestItems = children;

                //空白行
                ListViewItem4Log itemSpace = new ListViewItem4Log();
                this.lvwTests.Items.Add(itemSpace);

            }

            //自动调整列宽
            this.lvwTests.AutoResizeColumns(
                ColumnHeaderAutoResizeStyle.ColumnContent);
            this.lvwTests.EndUpdate();
            this.lvwTests.ItemChecked += this.lvwTests_ItemChecked;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.lblLogFileName.Text = logFileName;
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "txt|*.txt|log|*.log";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                logFileName = dlg.FileName;
                this.lblLogFileName.Text = logFileName;
            }
        }

        /// 设计器的列顺序有问题，所以手工调整了下
        /// </summary>
        private void ResetColumnOrders()
        {
            this.lvwTests.Columns.Clear();
            this.lvwTests.Columns.Add(this.Item);
            this.lvwTests.Columns.Add(this.Date);
            this.lvwTests.Columns.Add(this.Status);
            this.lvwTests.Columns.Add(this.SN);
        }
        #endregion


        /// <summary>
        /// 变量初始化
        /// </summary>
        private void InitValues()
        {
            this.listPasses = new List<Pass>();
            this.logFileTotalLinesGuess = this.GuessLogFileLineCount();
            this.readingLinesCount = 0;
        }

        #region Extract pass-test

        private async Task<IList<Pass>> ExtractPassesFromInputByRegex(string input)
        {
            List<Pass> passes = new List<Pass>();
            await Task.Run(() =>
            {
                foreach (Match m in Constants.regFindPasses.Matches(input))
                {
                    string g2 = m.Groups[2].Value;
                    Pass p = null;
                    if (g2.Contains(Constants.At))
                        p = new Pass(g2, m.Groups[1].Value, m.Groups[3].Value);
                    else//[]之间没有@的空Pass
                        p = new EmptyPass(input, m.Groups[1].Value, m.Groups[3].Value);
                    passes.Add(p);
                }
            });
            return passes;
        }
        #endregion Extract pass-test

        #region private methords
        private long GuessLogFileLineCount()
        {
            long len = new FileInfo(logFileName).Length;
            return (long)(len / Constants.fileLenth2LinesRate);
        }

        #endregion private methords

        private async void btnSave_Click(object sender, EventArgs e)
        {
            this.btnSave.Enabled = false;
            string re = await SaveModifiedLog();
            this.lblLogFileName.Text = re;
            this.btnSave.Enabled = true;
        }

        private async Task<string> SaveModifiedLog()
        {
            string outFileName = "";
            try
            {
                string temp = Path.GetTempFileName();
                outFileName = Path.Combine(Path.GetDirectoryName(logFileName),
                  Path.GetFileNameWithoutExtension(temp) + ".txt");
                using (StreamWriter sw = new StreamWriter(outFileName))
                {
                    await WriteCheckedTests(sw);
                }
            }
            catch (Exception ex)
            {
                return string.Format(ex.Message);
            }
            return outFileName;
        }

        private async Task WriteCheckedTests(StreamWriter sw)
        {
            await Task.Run(() =>
            {
                this.lvwTests.InvokeIfRequired((MethodInvoker)delegate
                {
                    foreach (var item4 in this.lvwTests.Items.OfType<ListViewItem4Log>()
                        .Where(x => x.IemLogType == ListViewItemLog4Type.PassStart && x.Checked))
                    {
                        Pass p = item4.Pass;
                        p.ListTests = item4.ChildrenTestItems
                            .Where(i => i.Checked).Select(x => x.Test).ToList();
                        sw.WriteLine(p);
                    }
                });
            });
        }

        #region check/uncheck

        private void lvwTests_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ListViewItem4Log item4 = e.Item as ListViewItem4Log;

            bool b = item4.Checked;
            this.lvwTests.ItemChecked -= this.lvwTests_ItemChecked;
            switch (item4.IemLogType)
            {
                case ListViewItemLog4Type.Empty:
                    item4.Checked = false;
                    break;
                case ListViewItemLog4Type.PassStart:
                case ListViewItemLog4Type.PassEnd:
                    item4.ParentPasses[0].Checked = b;
                    item4.ParentPasses[1].Checked = b;
                    foreach (var item in item4.ChildrenTestItems)
                        item.Checked = b;
                    break;
                case ListViewItemLog4Type.Test:
                default:
                    break;
            }
            this.lvwTests.ItemChecked += this.lvwTests_ItemChecked;

        }
        #endregion check/uncheck
    }
    public static class Extension
    {
        public static void InvokeIfRequired(this Control control, MethodInvoker action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();
        }
    }
}
