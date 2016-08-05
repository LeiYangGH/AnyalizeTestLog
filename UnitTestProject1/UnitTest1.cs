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
using System.Threading.Tasks;
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
        public void TestExtractOnePassFromInput()
        {
            var reader = new LogSubstringReader("");
            Pass pass = reader.ExtractOnePassBySubString(onePassTextWith2Tests);
            Assert.AreEqual("26-FEB-16  14:10:50", pass.StartDateString);
            Assert.AreEqual(new DateTime(2016, 2, 26, 14, 10, 50), pass.StartDate);
            Assert.AreEqual("26-FEB-16  14:52:14", pass.EndDate);
            Assert.AreEqual(2, pass.listTests.Count);
            Assert.AreEqual("E", pass.listTests[0].Status);
            Assert.AreEqual("SS160605E", pass.listTests[1].SN);
        }

        [TestMethod]
        public void TestSelectClonePassAndTests()
        {
            var reader = new LogSubstringReader("");
            Pass pass = reader.ExtractOnePassBySubString(onePassTextWith2Tests);
            Pass clone = pass.PassClonedWithBasicProperties;
            clone.listTests = pass.listTests.Where((x, i) => i == 0).ToList();
            Assert.AreEqual(1, clone.listTests.Count);
            clone.listTests[0].SN = "XXX";
            Assert.AreEqual(pass.listTests[0].SN, clone.listTests[0].SN);
        }

        [TestMethod]
        public void TestWriteKeepData()
        {
            string tempFile1 = Path.GetTempFileName();
            File.WriteAllText(tempFile1, onePassTextWith2Tests);

            var readProgress = new Progress<ReadProgress>();

            var reader1 = new LogSubstringReader(tempFile1);
            var task1 = reader1.ReadAndExtractPasses(readProgress);
            IList<Pass> passes1 = task1.Result;
            File.Delete(tempFile1);

            string tempFile2 = Path.GetTempFileName();
            var writer = new LogWriter(tempFile2);
            var tw = Task.Run(() => writer.SavePasses(passes1));
            Task.WaitAll(tw);

            var reader2 = new LogSubstringReader(tempFile2);
            var task2 = reader2.ReadAndExtractPasses(readProgress);
            IList<Pass> passes2 = task2.Result;
            File.Delete(tempFile2);

            Assert.AreEqual(passes1.Count, passes2.Count);
            for (int i = 0; i < passes1.Count; i++)
            {
                Assert.AreEqual(passes1[i].StartDateString, passes2[i].StartDateString);
                Assert.AreEqual(passes1[i].EndDate, passes2[i].EndDate);
                Assert.AreEqual(passes1[i].listTests.Count, passes2[i].listTests.Count);
                for (int j = 0; j < passes1[i].listTests.Count; j++)
                {
                    Assert.AreEqual(passes1[i].listTests[j].Date, passes2[i].listTests[j].Date);
                    Assert.AreEqual(passes1[i].listTests[j].SN, passes2[i].listTests[j].SN);
                    Assert.AreEqual(passes1[i].listTests[j].Status, passes2[i].listTests[j].Status);
                }
            }
        }

        [TestMethod]
        public void TestWriteNonEmptyLinesExactlySame()
        {
            string tempFile1 = Path.GetTempFileName();
            File.WriteAllText(tempFile1, onePassTextWith2Tests);

            var readProgress = new Progress<ReadProgress>();

            var reader1 = new LogSubstringReader(tempFile1);
            var task1 = reader1.ReadAndExtractPasses(readProgress);
            IList<Pass> passes1 = task1.Result;

            string tempFile2 = Path.GetTempFileName();
            var writer = new LogWriter(tempFile2);
            var tw = Task.Run(() => writer.SavePasses(passes1));
            Task.WaitAll(tw);

            string[] lines1 = File.ReadAllLines(tempFile1);
            string[] lines2 = File.ReadAllLines(tempFile2);

            File.Delete(tempFile1);
            File.Delete(tempFile2);

            string[] nonEmptyLines1 = lines1.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            string[] nonEmptyLines2 = lines2.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            Assert.IsTrue(nonEmptyLines1.Length > 0);
            Assert.AreEqual<int>(nonEmptyLines1.Length, nonEmptyLines2.Length);
            for (int i = 0; i < nonEmptyLines1.Length; i++)
            {
                Assert.AreEqual<string>(nonEmptyLines1[i], nonEmptyLines2[i]);
            }

            int minLinesCount = lines1.Length <= lines2.Length ? lines1.Length : lines2.Length;
            for (int i = 0; i < minLinesCount; i++)
            {
                Assert.AreEqual<string>(lines1[i], lines1[i]);
            }
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
