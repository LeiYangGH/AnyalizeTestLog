using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using LogProcessor;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        //string logFileName = Path.Combine(Environment.CurrentDirectory, @"input0.txt");
        string logFileName = Path.Combine(Environment.CurrentDirectory, @"input1.txt");
        //string logFileName = Path.Combine(Environment.CurrentDirectory, @"inputsubpass.txt");

        [TestMethod]
        public void TestListViewItemParent()
        {
            ListView lvw = new ListView();
            ListViewItem item = new ListViewItem();
            lvw.Items.Add(item);
            Console.WriteLine(item.ListView);
            Assert.AreEqual(lvw, item.ListView);
        }

        [TestMethod]
        public void TestCurrentDirectory()
        {
            string s = Environment.CurrentDirectory;
            Console.WriteLine(s);
            Console.WriteLine("ok");
        }

        [TestMethod]
        public void TestDevide()
        {
            int a = 3;
            int b = 6;
            double re = a / (double)b;
            Assert.AreEqual(0.5, re);
        }

        [TestMethod]
        public void TestMatchPassAndDate()
        {
            bool b = Constants.regFindPasses.IsMatch(onePassText);
            Assert.AreEqual(true, b);

            foreach (Match m in Constants.regFindPasses.Matches(onePassText))
            {
                Console.WriteLine("group1 = {0}", m.Groups[1].Value);
                Console.WriteLine("group3 = {0}", m.Groups[3].Value);
                //Console.WriteLine("group2 = {0}", m.Groups[2].Value);
                Assert.AreEqual("26-FEB-16  14:10:50", m.Groups[1].Value);
                Assert.AreEqual("26-FEB-16  14:52:14", m.Groups[3].Value);
            }
        }

        [TestMethod]
        public void TestMatchSN()
        {
            string sn = Test.ExtractSNFormATest(onePassText);
            Console.WriteLine(sn);
            Assert.AreEqual("SS160605E", sn);
        }

        static string onePassText = @"
S:\Projects\PR\PR11274_Rev01\PR11274.obc[26-FEB-16  14:10:50
@26-FEB-16  14:12:16

PHILIPS P/N = 1118921

PRODUCT = PR11274

OPERATOR = 24704

STATUS = ENGINEERING

TESTER ID = G016
?26-FEB-16  14:24:18

@26-FEB-16  14:45:13 SN SS160605E

PHILIPS P/N = 1118921

PRODUCT = PR11274

OPERATOR = 24704

STATUS = ENGINEERING

TESTER ID = G016
R1=1.00466K(950,1.05K)R 
PS5_OFF=(3.1,3.5)V 
PS5_OFF=-3.206191U(-100.000001M,500M)V 
VBATTZ=(-100.000001M,500M)V 
PS_OFF=(22.799999,25.200001)V 
PS_OPEN=(-39.999999M,39.999999M)V 
?26-FEB-16  14:52:09

]26-FEB-16  14:52:14";
    }
}
