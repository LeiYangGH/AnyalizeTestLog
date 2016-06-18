using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace LogProcessor
{
    internal class ListViewItem4Log : ListViewItem
    {
        public ListViewItem4Log()
        {
            this.IemLogType = ListViewItemLog4Type.Empty;
            this.Checked = false;
        }

        public ListViewItem4Log(Pass pass, bool start)
        {
            this.Text = start ? "【" : "】";
            this.SubItems.Add(pass.StartDateString);
            this.Pass = pass;
            this.IemLogType = start ?
                ListViewItemLog4Type.PassStart : ListViewItemLog4Type.PassEnd;
            this.Checked = true;
        }

        public ListViewItem4Log(Pass pass, Test test)
        {
            this.Text = (pass.ListTests.IndexOf(test) + 1).ToString();
            this.ToolTipText = test.ToString();
            this.SubItems.Add(test.Date);
            this.SubItems.Add(test.Status);
            this.SubItems.Add(test.SN);
            this.Pass = pass;
            this.Test = test;
            this.IemLogType = ListViewItemLog4Type.Test;
            this.Checked = true;
        }

        public IList<ListViewItem4Log> ParentPasses { get; set; }
        public IList<ListViewItem4Log> ChildrenTestItems { get; set; }

        public ListViewItemLog4Type IemLogType
        {
            get;
            private set;
        }

        public Pass Pass
        {
            get;
            private set;
        }

        public Test Test
        {
            get;
            private set;
        }
    }

    public enum ListViewItemLog4Type
    {
        PassStart,
        PassEnd,
        Test,
        Empty
    }
}
