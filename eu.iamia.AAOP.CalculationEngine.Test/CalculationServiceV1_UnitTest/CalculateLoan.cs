using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable InconsistentNaming

namespace eu.iamia.AAOP.CalculationEngine.Test.CalculationServiceV1_UnitTest
{
    using eu.iamia.AAOP.CalculationEngine.CalculationV1;
    using eu.iamia.AAOP.CalculationEngine.Models;
    using eu.iamia.AAOP.ISP.Models;

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
            TestTarget  = new CalculationServiceV1();

            LoanSettings = new LoanSettings
                               {
                                   Lånebeøb = 50000m,
                                   Løbetid = 5 * 12,
                                   PålydendeRente = 0.08m,
                                   Startomkostning = 500m
                               };

            ExpectedValue = new LoanCalculations { ÅOP = 8.701m, Ydelse = 1013.82m, DebitorRente = 0.083m };
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
        public void CalculateLoan_Test01()
        {
            TestTarget.Init(LoanSettings);

            TestTarget.CalculateLoan();

            Assert.AreEqual(ExpectedValue.Ydelse, TestTarget.LoanCalculations.Ydelse, "Ydelse");
            Assert.AreEqual(ExpectedValue.DebitorRente, TestTarget.LoanCalculations.DebitorRente, "DebitorRente");
            Assert.AreEqual(ExpectedValue.ÅOP, TestTarget.LoanCalculations.ÅOP, "ÅOP");

        }


    }
}
