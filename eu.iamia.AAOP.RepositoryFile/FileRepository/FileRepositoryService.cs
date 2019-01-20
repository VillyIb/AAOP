using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eu.iamia.AAOP.RepositoryFile.FileRepository
{
    using System.IO;
    using System.Runtime.InteropServices;

    using eu.iamia.AAOP.CalculationEngine.Models;
    using eu.iamia.AAOP.ISP.Models;
    using eu.iamia.AAOP.ISP.Services;
    using eu.iamia.Util;

    public class FileRepositoryService : IRepositoryService
    {

        /// <summary>
        /// NOTE HARDCODED PATH to repository
        /// </summary>
        private const string Repository = @"C:\Development\FinancialCalculator\Repository";


        private List<ILoanStorageMetadata> FileList { get; set; }

        private void LoadFileList()
        {
            var di = new DirectoryInfo(Repository);

            var files = di.GetFiles("*.xml");
            FileList = new List<ILoanStorageMetadata>(files.Length);

            foreach (var file in files.OrderBy(t => t.Name))
            {
                var t1 = new LoanStorageMetadata
                {
                    Id = FileList.Count+1
                    , Name = file.Name.Replace(".xml","")
                };
                FileList.Add(t1);                      
            }

            Console.WriteLine("Found {0} files", FileList.Count);

        }


        public void Create(out ILoanStorageMetadata loanStorageMetadata, ILoanStorage loan)
        {
            var filename = loan.LoanStorageMetadata.Name + ".xml";
            var fullPath = System.IO.Path.Combine(Repository, filename);
            Console.WriteLine("Create: '{0}'   ", fullPath);
            var fi = new FileInfo(fullPath);
            if (fi.Exists) { throw new ArgumentException(String.Format("File '{0}' exists", fi.FullName)); }

            var fs = fi.OpenWrite();

            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                sw.WriteLine(loan.LoanSettings.SerializeToXmlUtf8());
            }

            this.LoadFileList();
            loanStorageMetadata = FileList.First(t => t.Name.Equals(loan.LoanStorageMetadata.Name));
        }


        public void Read(out ILoanSettings loan, ILoanStorageMetadata loanStorageMetadata)
        {
            if (null == FileList || 0 == FileList.Count) { throw new ApplicationException("FileList not loaded"); }

            var file = FileList.FirstOrDefault(t => loanStorageMetadata.Id == t.Id);
            if (null == file) { throw new ArgumentOutOfRangeException(nameof(loanStorageMetadata), String.Format("File with Id: {0} not found", loanStorageMetadata.Id)); }

            var fullPath = Path.Combine(Repository, file.Name + ".xml");

            var fi = new FileInfo(fullPath);
            var fs = fi.OpenRead();

            using (var st = new StreamReader(fs))
            {

                var t1 = st.ReadToEnd();

                LoanSettings t2;

                if (XmlUtils.TryParse(t1, out t2))
                {
                    loan = t2;
                    return;
                }
            }

            throw new ArgumentException("Unable to read Loan");

        }


        internal void RepositoryClean()
        {
            this.LoadFileList();


            foreach (var file in FileList)
            {
                var fullPath = Path.Combine(Repository, file.Name + ".xml");
                var fi = new FileInfo(fullPath);
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
        }


        public void Read(out IEnumerable<ILoanStorageMetadata> loanStorageMetadataList)
        {
            LoadFileList();
            loanStorageMetadataList = FileList;
        }
    }
}
