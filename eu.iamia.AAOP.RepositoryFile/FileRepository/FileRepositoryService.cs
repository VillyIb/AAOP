using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eu.iamia.AAOP.RepositoryFile.FileRepository
{
    using eu.iamia.AAOP.ISP.Models;
    using eu.iamia.AAOP.ISP.Services;
    public class FileRepositoryService : IRepositoryService
    {
        public void Create(out ILoanStorageMetadata loanStorageMetadata, ILoanStorage loan)
        {
            throw new NotImplementedException();
        }


        public void Read(out ILoanStorage loan, ILoanStorageMetadata loanStorageMetadata)
        {
            throw new NotImplementedException();
        }


        public void Read(out IEnumerable<ILoanStorageMetadata> loanStorageMetadataList)
        {
            throw new NotImplementedException();
        }
    }
}
