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
using LogProcessor;
namespace LogProcessorWinForm
{
    public partial class FrmMain : Form
    {
        #region Fields
        private string logFileName;
        private List<Pass> listPasses;
        #endregion Fields

        public FrmMain()
        {
            InitializeComponent();
        }

        #region Properties

        #endregion Properties

        //点击Extract按钮后会触发，整个函数运行完则获取到所有数据并显示
        private async Task<long> StartReadAndExtract()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            ILogReader logReader = new LogSubstringReader(this.logFileName);

            var readProgress = new EventProgress<ReadProgress>();
            readProgress.ProgressChanged += (s, e) =>
            {
                this.UpdateReadProgress(e.Value);
            };

            //主要耗时部分
            var Passes = await logReader.ReadAndExtractPasses(readProgress);

            this.listPasses = Passes.ToList();

            this.DisplayPasses(this.listPasses);

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }


        #region Form UI
        private async void btnExtractTests_Click(object sender, EventArgs e)
        {
            this.btnExtractTests.Enabled = false;
            Properties.Settings.Default.LastLogFileName = this.logFileName;
            Properties.Settings.Default.Save();
            long sec = await this.StartReadAndExtract();

            this.btnExtractTests.Enabled = true;
            this.statusLblMsg.Text = string.Format("Time used（ms）： {0}", sec);
            if (this.listPasses.Count > 0)
                this.btnSave.Enabled = true;
        }

        //更新读文件进度
        private void UpdateReadProgress(ReadProgress rProgress)
        {
            this.progressBar.Value = rProgress.ReadingPercent;
            this.statusLblMsg.Text = rProgress.Message;
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
                foreach (var t in p.listTests)
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
            ResetColumnOrders();
            this.logFileName = Properties.Settings.Default.LastLogFileName;
            this.lblLogFileName.Text = this.logFileName;
        }

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = Constants.logExtFilter;
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



        #region private methords
        #endregion private methords


        private string ChooseSaveFileName()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = Constants.logExtFilter;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (this.logFileName == dlg.FileName)
                {
                    MessageBox.Show("You should not overwrite the input log!");
                    return null;
                }
                return dlg.FileName;
            }
            return null;
        }

        /// <summary>
        /// 获取勾选的Passes
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        private IList<Pass> GetCheckedPasses()
        {
            List<Pass> lstP = new List<Pass>();
            foreach (var item4 in this.lvwTests.Items.OfType<ListViewItem4Log>()
                .Where(x => x.IemLogType == ListViewItemLog4Type.PassStart && x.Checked))
            {
                Pass p = item4.Pass.PassClonedWithBasicProperties;
                p.listTests = item4.ChildrenTestItems
                    .Where(i => i.Checked).Select(x => x.Test).ToList();
                lstP.Add(p);
            }
            return lstP;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            string fileName = ChooseSaveFileName();
            if (string.IsNullOrWhiteSpace(fileName))
                return;
            this.btnSave.Enabled = false;

            this.statusLblMsg.Text = "Saving...";
            var passes = this.GetCheckedPasses();
            string re = await (new LogWriter(fileName)).SavePasses(passes);
            this.statusLblMsg.Text = re;

            this.btnSave.Enabled = true;

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
