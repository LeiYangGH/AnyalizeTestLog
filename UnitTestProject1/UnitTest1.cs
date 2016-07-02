using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using LogProcessor;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
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
        public void TestExtractOneTest()
        {
            Test test = new Test(oneFailTestTextWithSN);
            Assert.AreEqual("26-FEB-16  14:12:16", test.DateString);
            Assert.AreEqual("F", test.Status);
            Assert.AreEqual("SS160605E", test.SN);
        }

        [TestMethod]
        public void TestExtractEmptyPassFromInput()
        {
            var reader = new LogSubstringReader("");
            Pass pass = reader.ExtractOnePassBySubString(oneEmptyPass);
            Assert.AreEqual("26-FEB-16  15:04:49", pass.StartDateString);
            Assert.AreEqual("26-FEB-16  18:19:06", pass.EndDate);
            Assert.AreEqual(0, pass.listTests.Count);
        }

        [TestMethod]
        public void TestExtractOnePassFromInput()
        {
            var reader = new LogSubstringReader("");
            Pass pass = reader.ExtractOnePassBySubString(onePassTextWith2Tests);
            Assert.AreEqual("26-FEB-16  14:10:50", pass.StartDateString);
            Assert.AreEqual("26-FEB-16  14:52:14", pass.EndDate);
            Assert.AreEqual(2, pass.listTests.Count);
            Assert.AreEqual("E", pass.listTests[0].Status);
            Assert.AreEqual("SS160605E", pass.listTests[1].SN);
        }

        static string oneEmptyPass = @"
S:\Projects\PR\PR11274_Rev01\PR11274.obc[26-FEB-16  15:04:49
]26-FEB-16  18:19:06
";

        static string onePassTextWith2Tests = @"
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

        static string oneFailTestTextWithSN = @"26-FEB-16  14:12:16 SN SS160605E

PHILIPS P/N = 1118921

PRODUCT = PR11274

OPERATOR = 24704

STATUS = ENGINEERING

TESTER ID = G016
/26-FEB-16  14:24:18
";
    }
}
