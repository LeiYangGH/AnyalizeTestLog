using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LogProcessor;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Collections.Generic;
namespace LogProcessorWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private static string curDir = Environment.CurrentDirectory;


        #endregion Fields

        public MainViewModel()
        {
            MessengerInstance.Register<PassViewModel>(this, (p) =>
            {
                this.RaisePropertyChanged(() => this.PassesCntMsg);
            });
            this.LogFileName = Properties.Settings.Default.LastLogFileName;
        }

        /// <summary>
        /// WPF要求后台的集合类型为ObservableCollection
        /// </summary>
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
            long sec = await this.StartReadAndExtract();
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

        /// <summary>
        /// 保存勾选的Passes和Tests
        /// </summary>
        /// <returns></returns>
        private async Task Save()
        {
            string saveFileName = ChooseSaveFileName();
            if (string.IsNullOrWhiteSpace(saveFileName))
                return;
            this.Msg = "Saving...";
            var passes = this.GetCheckedPasses();
            var writer = new LogWriter(saveFileName);
            string re = await writer.SavePasses(passes);
            this.Msg = re;
        }
        #endregion commands

        //点击Extract按钮后会触发，整个函数运行完则获取到所有数据并显示
        private async Task<long> StartReadAndExtract()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            ILogReader logReader = new LogSubstringReader(this.LogFileName);

            //主要耗时部分
            var Passes = await logReader.ReadAndExtractPasses(new Progress<ReadProgress>(p => this.UpdateReadProgress(p)));

            //WPF显示的需要，把List<Pass>转化为ObservableCollection<PassViewModel>
            this.ObsPasses = new ObservableCollection<PassViewModel>(
                Passes
                //.OrderBy(x => x.StartDate)//不改变写时顺序
                .Select(x => new PassViewModel(x)));

            this.RaisePropertyChanged(() => this.PassesCntMsg);

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        #region Form UI

        //更新读文件进度
        private void UpdateReadProgress(ReadProgress rProgress)
        {
            this.Progress = rProgress.ReadingPercent;
            this.Msg = rProgress.Message;
        }

        #endregion



        /// <summary>
        /// 获取勾选的Passes
        /// </summary>
        /// <param name="sw"></param>
        /// <returns></returns>
        private IEnumerable<Pass> GetCheckedPasses()
        {
            return this.ObsPasses.Where(p => p.IsChecked ?? false)
                .Select(p => p.PassClonedWithCheckedTests);
        }
    }
}
