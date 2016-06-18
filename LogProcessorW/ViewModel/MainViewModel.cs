using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LogProcessor;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using System.Windows;
namespace LogProcessorW.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        #region Fields

        //默认的log文件
        private static string curDir = Environment.CurrentDirectory;
        private long logFileTotalLinesGuess;
        private int readingLinesCount;

        #endregion Fields

        public MainViewModel()
        {
            MessengerInstance.Register<PassViewModel>(this, (p) =>
            {
                this.RaisePropertyChanged(() => this.PassesCntMsg);
            });
            this.LogFileName = Properties.Settings.Default.LastLogFileName;
        }

        private ObservableCollection<PassViewModel> obsPasses;
        public ObservableCollection<PassViewModel> ObsPasses
        {
            get
            {
                return this.obsPasses;
            }
            set
            {
                if (this.obsPasses != value)
                {
                    this.obsPasses = value;
                    this.RaisePropertyChanged(() => this.ObsPasses);
                }
            }
        }

        public string PassesCntMsg
        {
            get
            {
                if (this.ObsPasses == null)
                    return "0 / 0";
                int total = this.ObsPasses.Count;
                int chk = this.ObsPasses.Count(x => x.IsChecked ?? false);
                return string.Format("{0} / {1}", chk, total);
            }
        }

        private string logFileName;
        public string LogFileName
        {
            get
            {
                return this.logFileName;
            }
            set
            {
                if (this.logFileName != value)
                {
                    this.logFileName = value;
                    this.RaisePropertyChanged(() => this.LogFileName);
                }
            }
        }

        private int progress;
        public int Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                if (this.progress != value)
                {
                    this.progress = value;
                    this.RaisePropertyChanged(() => this.Progress);
                }
            }
        }


        private string msg;
        public string Msg
        {
            get
            {
                return this.msg;
            }
            set
            {
                if (this.msg != value)
                {
                    this.msg = value;
                    this.RaisePropertyChanged(() => this.Msg);
                }
            }
        }

        private string perf;
        public string Perf
        {
            get
            {
                return this.perf;
            }
            set
            {
                if (this.perf != value)
                {
                    this.perf = value;
                    this.RaisePropertyChanged(() => this.Perf);
                }
            }
        }
        #region commands
        private bool isExtracting;
        private RelayCommand extractCommand;

        public RelayCommand ExtractCommand
        {
            get
            {
                return extractCommand
                  ?? (extractCommand = new RelayCommand(
                    async () =>
                    {
                        if (isExtracting)
                        {
                            return;
                        }

                        isExtracting = true;
                        ExtractCommand.RaiseCanExecuteChanged();

                        await Extract();

                        isExtracting = false;
                        ExtractCommand.RaiseCanExecuteChanged();
                    },
                    () => !isExtracting));
            }
        }
        private async Task Extract()
        {
            if (!File.Exists(this.LogFileName))
            {
                return;
            }
            Properties.Settings.Default.LastLogFileName = this.LogFileName;
            Properties.Settings.Default.Save();
            long sec = await this.Go();
            this.Perf = string.Format("Time used（ms）： {0}", sec);
        }

        private bool isOpening;
        private RelayCommand openCommand;

        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand
                  ?? (openCommand = new RelayCommand(
                    async () =>
                    {
                        if (isOpening)
                        {
                            return;
                        }

                        isOpening = true;
                        OpenCommand.RaiseCanExecuteChanged();

                        await Open();

                        isOpening = false;
                        OpenCommand.RaiseCanExecuteChanged();
                    },
                    () => !isOpening));
            }
        }
        private async Task Open()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = Constants.logExtFilter;
            if (dlg.ShowDialog() ?? false)
            {
                this.LogFileName = dlg.FileName;
            }
        }

        private bool isSaving;
        private RelayCommand saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand
                  ?? (saveCommand = new RelayCommand(
                    async () =>
                    {
                        if (isSaving)
                        {
                            return;
                        }

                        isSaving = true;
                        SaveCommand.RaiseCanExecuteChanged();

                        await Save();

                        isSaving = false;
                        SaveCommand.RaiseCanExecuteChanged();
                    },
                    () => !isSaving));
            }
        }

        private string ChooseSaveFileName()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = Constants.logExtFilter;
            if (dlg.ShowDialog() ?? false)
            {
                if (this.LogFileName == dlg.FileName)
                {
                    MessageBox.Show("You should not overwrite the input log!");
                    return null;
                }
                return dlg.FileName;
            }
            return null;
        }

        private async Task Save()
        {
            string fileName = ChooseSaveFileName();
            if (string.IsNullOrWhiteSpace(fileName))
                return;
            string re = await SaveModifiedLog(fileName);
            this.Msg = string.Format(re);

        }
        #endregion commands


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

            this.ObsPasses = new ObservableCollection<PassViewModel>(
                rePasses.Select(x => new PassViewModel(x)));

            this.RaisePropertyChanged(() => this.PassesCntMsg);

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
            StringBuilder sbLines4Pass = new StringBuilder();
            foreach (string line in File.ReadLines(logFileName, Encoding.UTF8))
            {
                this.readingLinesCount++;
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                sbLines4Pass.AppendLine(line);
                if (line.Contains(Constants.passEndString))
                {
                    IList<Pass> passes = await ExtractPassesFromInputByRegex(sbLines4Pass.ToString());
                    lst.AddRange(passes);
                    sbLines4Pass.Clear();
                    if (progress != null)
                        progress.Report(this.readingLinesCount);
                }
            }
            return lst;
        }
        #endregion Read log file


        #region Form UI

        //更新读文件进度
        private void UpdateReadProgress(long reading)
        {
            if (reading > this.logFileTotalLinesGuess)
                reading = logFileTotalLinesGuess;

            this.Progress = (int)(reading / (double)this.logFileTotalLinesGuess * 100d);
            this.Msg = string.Format("{0} lines read", reading);
        }

        #endregion


        /// <summary>
        /// 变量初始化
        /// </summary>
        private void InitValues()
        {
            this.logFileTotalLinesGuess = this.GuessLogFileLineCount();
            this.readingLinesCount = 0;
        }

        #region Extract pass-test

        private async Task<IList<Pass>> ExtractPassesFromInputByRegex(string input)
        {
            List<Pass> passes = new List<Pass>();
            await Task.Run(() =>
            {
                int passStartSymbolLoc = input.IndexOf(Constants.passStartString);
                if (passStartSymbolLoc > 0)
                {
                    int passEndSymbolLoc = input.LastIndexOf(Constants.passEndString);
                    string g1 = input.Substring(passStartSymbolLoc + 1, 19);
                    string g2 = input.Substring(passStartSymbolLoc + 1, passEndSymbolLoc - passStartSymbolLoc - 1);
                    string g3 = input.Substring(passEndSymbolLoc + 1, 19);
                    Debug.Assert(passStartSymbolLoc > 0, passStartSymbolLoc.ToString());
                    Debug.Assert(passEndSymbolLoc - passStartSymbolLoc >= Constants.dateStringLenth);
                    Pass p;
                    if (passEndSymbolLoc - passStartSymbolLoc > Constants.minPassSymbolDistance)
                        p = new Pass(g2, g1, g3);
                    else//[]之间没有@的空Pass
                        p = new EmptyPass(input, g1, g3);
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



        private async Task<string> SaveModifiedLog(string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8))
                {
                    await WriteCheckedTests(sw);
                }
            }
            catch (Exception ex)
            {
                return string.Format(ex.Message);
            }

            return fileName;
        }

        private async Task WriteCheckedTests(StreamWriter sw)
        {
            await Task.Run(() =>
            {
                foreach (PassViewModel passVM in this.ObsPasses)
                {
                    if (passVM.IsChecked ?? false)
                    {
                        Pass p = passVM.pass;
                        p.ListTests = passVM.ObsTests.Where(x => x.IsChecked ?? false)
                            .Select(x => x.test).ToList();
                        sw.WriteLine(p);
                    }
                }
            });
        }
    }
}
