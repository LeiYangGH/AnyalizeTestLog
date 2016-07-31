using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using LogProcessor;
using System.Collections.ObjectModel;
namespace LogProcessorWPF.ViewModel
{
    public class PassViewModel : ViewModelBase
    {
        public readonly Pass pass;

        public PassViewModel(Pass pass)
        {
            this.pass = pass;
            this.IsChecked = true;
            this.StartDateString = pass.StartDateString;
            this.StartDate = pass.StartDate;
            this.EndDate = pass.EndDate;
            this.ConstructObsTests();
            this.HasTests = this.ObsTests.Count > 0;
            MessengerInstance.Register<TestViewModel>(this, (t) =>
            {
                if (this.ObsTests.Contains(t))
                {
                    this.isChecked = this.ObsTests.Any(x => x.IsChecked ?? false);
                    this.RaisePropertyChanged(() => this.IsChecked);
                    this.RaisePropertyChanged(() => this.TestsCntMsg);
                }
            });
        }

        private void ConstructObsTests()
        {
            this.ObsTests = new ObservableCollection<TestViewModel>(
                pass.listTests.Select(x => new TestViewModel(x)));
        }

        /// <summary>
        /// 根据勾选Test的情况重新组装新的Pass供保存用
        /// </summary>
        public Pass PassClonedWithCheckedTests
        {
            get
            {
                Pass clone = this.pass.PassClonedWithBasicProperties;
                if (clone.HasTests)
                    clone.listTests = this.ObsTests
                        .Where(t => t.IsChecked ?? false)
                        .Select(x => x.test).ToList();
                return clone;
            }
        }

        private ObservableCollection<TestViewModel> obsTests;
        public ObservableCollection<TestViewModel> ObsTests
        {
            get
            {
                return this.obsTests;
            }
            set
            {
                if (this.obsTests != value)
                {
                    this.obsTests = value;
                    this.RaisePropertyChanged(() => this.ObsTests);
                }
            }
        }

        private bool? isChecked;
        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.RaisePropertyChanged(() => this.IsChecked);
                    this.RaisePropertyChanged(() => this.TestsCntMsg);
                    if (this.ObsTests != null)
                        foreach (TestViewModel t in this.ObsTests
                            .Where(t => t.IsChecked != value))
                        {
                            t.IsChecked = value;
                        }
                    MessengerInstance.Send<PassViewModel>(this);
                }
            }
        }



        public string TestsCntMsg
        {
            get
            {
                int total = this.ObsTests.Count;
                int chk = this.ObsTests.Count(x => x.IsChecked ?? false);
                return string.Format("{0}/{1}", chk, total);
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }
            set
            {
                if (this.startDate != value)
                {
                    this.startDate = value;
                    this.RaisePropertyChanged(() => this.StartDate);
                }
            }
        }

        private string startDateString;
        public string StartDateString
        {
            get
            {
                return this.startDateString;
            }
            set
            {
                if (this.startDateString != value)
                {
                    this.startDateString = value;
                    this.RaisePropertyChanged(() => this.StartDateString);
                }
            }
        }

        private string endDate;
        public string EndDate
        {
            get
            {
                return this.endDate;
            }
            set
            {
                if (this.endDate != value)
                {
                    this.endDate = value;
                    this.RaisePropertyChanged(() => this.EndDate);
                }
            }
        }

        public bool HasTests
        {
            get;
            set;
        }
    }
}
