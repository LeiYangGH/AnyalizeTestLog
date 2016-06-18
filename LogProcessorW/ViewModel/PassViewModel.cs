﻿using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using LogProcessor;
using System.Collections.ObjectModel;
namespace LogProcessorW.ViewModel
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
            this.ObsTests = new ObservableCollection<TestViewModel>(
               pass.ListTests.OrderBy(x => x.Date).Select(x => new TestViewModel(x, this)));
            this.HasTests = this.obsTests.Count > 0;
            MessengerInstance.Register<TestViewModel>(this, (t) =>
            {
                if (this.ObsTests.Contains(t))
                    this.RaisePropertyChanged(() => this.TestsCntMsg);
            });
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
                        foreach (TestViewModel t in this.ObsTests)
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
