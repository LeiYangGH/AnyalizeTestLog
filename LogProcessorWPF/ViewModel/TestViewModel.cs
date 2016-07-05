using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LogProcessor;
using System;
using System.Threading.Tasks;
using System.Windows.Documents;
namespace LogProcessorWPF.ViewModel
{
    public class TestViewModel : ViewModelBase
    {
        public readonly Test test;

        public TestViewModel(Test test)
        {
            this.test = test;
            this.IsChecked = true;
            this.DateString = test.DateString;
            this.Date = test.Date;
            this.Status = test.Status;
            this.SN = test.SN;
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
                    MessengerInstance.Send<TestViewModel>(this);
                }
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                if (this.date != value)
                {
                    this.date = value;
                    this.RaisePropertyChanged(() => this.Date);
                }
            }
        }

        private string dateString;
        public string DateString
        {
            get
            {
                return this.dateString;
            }
            set
            {
                if (this.dateString != value)
                {
                    this.dateString = value;
                    this.RaisePropertyChanged(() => this.DateString);
                }
            }
        }

        private string status;
        public string Status
        {
            get
            {
                return this.status;
            }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.RaisePropertyChanged(() => this.Status);
                }
            }
        }

        private string sn;
        public string SN
        {
            get
            {
                return this.sn;
            }
            set
            {
                if (this.sn != value)
                {
                    this.sn = value;
                    this.RaisePropertyChanged(() => this.SN);
                }
            }
        }

        private string details;
        public string Details
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.details))
                    this.details = this.test.ToString();
                return this.details;
            }
        }

        private bool isShowingDetails;
        private RelayCommand showDetailsCommand;
        public RelayCommand ShowDetailsCommand
        {
            get
            {
                return showDetailsCommand
                  ?? (showDetailsCommand = new RelayCommand(
                    async () =>
                    {
                        if (isShowingDetails)
                        {
                            return;
                        }

                        isShowingDetails = true;
                        ShowDetailsCommand.RaiseCanExecuteChanged();

                        await ShowDetails();

                        isShowingDetails = false;
                        ShowDetailsCommand.RaiseCanExecuteChanged();
                    },
                    () => !isShowingDetails));
            }
        }
        private async Task ShowDetails()
        {
            FlowDocument doc = new FlowDocument();
            Paragraph p = new Paragraph(new Run(this.Details));
            p.FontSize = 16;
            doc.Blocks.Add(p);
            TestDetailsWindow win = new TestDetailsWindow();
            win.Viewer.Document = doc;
            win.ShowDialog();
        }
    }
}
