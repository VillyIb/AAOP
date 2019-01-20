using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable InconsistentNaming

namespace eu.iamia.AAOP.RepositoryFile.Test
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using eu.iamia.AAOP.CalculationEngine.Models;
    using eu.iamia.AAOP.ISP.Models;
    using eu.iamia.AAOP.RepositoryFile.FileRepository;

    [TestClass]
    public class RepositoryFile_Test
    {
        private FileRepositoryService TestTarget { get; set; }


        private ILoanSettings LoanSettings { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext tx)
        {
            new FileRepositoryService().RepositoryClean();
        }


        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestTarget = new FileRepositoryService();

            LoanSettings = new LoanSettings
            {
                Lånebeøb = 50000m,
                Løbetid = 5 * 12,
                PålydendeRente = 0.08m,
                Startomkostning = 500m
            };

        }


        [TestMethod]
        public void RepositoryFile_Save1()
        {
            Thread.Sleep(2000);

            ILoanStorageMetadata lsmOut;

            var ls = new LoanStorage
            {
                LoanSettings = LoanSettings,
                LoanStorageMetadata = new LoanStorageMetadata { Name = "Test1" }
            };



            TestTarget.Create(out lsmOut, ls);

            Assert.AreEqual(ls.LoanStorageMetadata.Name, lsmOut.Name);

        }


        [TestMethod]
        public void RepositoryFile_Save2()
        {
            Thread.Sleep(2000);

            ILoanStorageMetadata lsmOut;

            var ls = new LoanStorage
            {
                LoanSettings = LoanSettings,
                LoanStorageMetadata = new LoanStorageMetadata { Name = "Test2" }
            };



            TestTarget.Create(out lsmOut, ls);

            Assert.AreEqual(ls.LoanStorageMetadata.Name, lsmOut.Name);

        }


        [TestMethod]
        public void RepositorFile_Read2()
        {
            Thread.Sleep(2000);

            IEnumerable<ILoanStorageMetadata> loanStorageMetadataList;

            TestTarget.Read(out loanStorageMetadataList);

            ILoanSettings loan;
            TestTarget.Read(out loan, loanStorageMetadataList.First(t =>  "Test2".Equals(t.Name, StringComparison.InvariantCultureIgnoreCase) ));

            Assert.IsNotNull(loan);

        }


    }
}
