using System;
using System.Text;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable InconsistentNaming

namespace eu.iamia.AAOP.CalculationEngine.Test.CalculationServiceV1_UnitTest
{
    using System.IO;

    using CalculationV1;
    using Models;
    using ExcelExport;
    using eu.iamia.AAOP.ISP.Models;
    using ISP.Services;
    using Util;

    /// <summary>
    /// Summary description for CalculateLoan
    /// </summary>
    [TestClass]
    public class CalculateLoan
    {
        public CalculateLoan()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private CalculationServiceV1 TestTarget { get; set; }

        private ILoanSettings LoanSettings { get; set; }

        private ILoanCalculations ExpectedValue { get; set; }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestTarget = new CalculationServiceV1();

            LoanSettings = new LoanSettings
            {
                Lånebeøb = 50000m,
                Løbetid = 5 * 12,
                PålydendeRente = 0.08m,
                Startomkostning = 500m
            };

            ExpectedValue = new LoanCalculations { ÅOP = 0.08760m, Ydelse = 1013.82m, DebitorRente = 0.083m };
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// First unit test - check init works.
        /// </summary>
        [TestMethod]
        public void CalculateLoan_Init()
        {
            TestTarget.Init(LoanSettings);

            Assert.IsNotNull(TestTarget.LoanSettings);

        }

        [TestMethod]
        public void CalculateLoan_Ydelse()
        {
            TestTarget.Init(LoanSettings);

            TestTarget.CalculateYdelse();

            Assert.AreEqual(ExpectedValue.Ydelse, TestTarget.LoanCalculations.Ydelse, "Ydelse");
        }


        [TestMethod]
        public void CalculateLoan_DebitorRente()
        {
            TestTarget.Init(LoanSettings);

            TestTarget.CalculateDebitorRente();

            Assert.AreEqual(ExpectedValue.DebitorRente, TestTarget.LoanCalculations.DebitorRente, "DebitorRente");
        }


        [TestMethod]
        public void CalculateLoan_AAOP()
        {
            TestTarget.Init(LoanSettings);

            TestTarget.CalculateYdelse();
            TestTarget.CalculateAAOP();

            Assert.AreEqual(ExpectedValue.ÅOP, TestTarget.LoanCalculations.ÅOP, "ÅOP");
        }



        [TestMethod]
        public void CalculateLoan_Rente()
        {

            var rente = TestTarget.Rente(50000d, -1013.82d, 60);
            Assert.AreEqual(Math.Round(0.08d / 12, 5), Math.Round(rente, 5), "Rente");
        }

        [TestMethod]
        public void CalculateLoan_MainTest()
        {
            var testTarget = TestTarget as IAAOPCalculationService;

            testTarget.Init(LoanSettings);
            testTarget.CalculateLoan();

            Assert.AreEqual(ExpectedValue.ÅOP, TestTarget.LoanCalculations.ÅOP, "ÅOP");
        }


        [TestMethod]
        public void CalculateLoan_PaymentPlan()
        {
            var testTarget = TestTarget as IAAOPCalculationService;

            testTarget.Init(LoanSettings);
            testTarget.CalculateLoan();


            var ppl = testTarget.PaymentPlan.PaymentDetailList.ToList();

            var toskip = ppl.Count - 3;

            var pps = ppl.Take(3).Union(ppl.Skip(toskip).Take(3));

            foreach (var row in pps)
            {
                Console.WriteLine("{0}", row.SerializeToXml());
            }

            Assert.AreEqual(ExpectedValue.ÅOP, TestTarget.LoanCalculations.ÅOP, "ÅOP");
        }



        [TestMethod]
        public void CalculateLoan_Export()
        {
            var testTarget = TestTarget as IAAOPCalculationService;

            testTarget.Init(LoanSettings);
            testTarget.CalculateLoan();

            // ATTENTION - HARDCODED path - directory MUST exist.

            var filename = @"C:\Development\FinancialCalculator\ExportData\Annuitet.xml";

            var fi = new FileInfo(filename);
            var fs = fi.OpenWrite();

            var converter = new WebExportToExcel();
            converter.Init();

            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                converter.Convert(sw, testTarget.PaymentPlan.PaymentDetailList);
            }
        }

    }
}
