using System.Collections.Generic;

namespace eu.iamia.AAOP.ISP.Services
{
    using eu.iamia.AAOP.ISP.Models;

    public interface IRepositoryService
    {
        /// <summary>
        /// Creates new loan in repository.
        /// </summary>
        /// <param name="loanStorageMetadata"></param>
        /// <param name="loan"></param>
        void Create(out ILoanStorageMetadata loanStorageMetadata, ILoanStorage loan);


        /// <summary>
        /// Reads the Loan specified by LoanStorageMetadata from repositroy
        /// </summary>
        /// <param name="loan"></param>
        /// <param name="loanStorageMetadata"></param>
        void Read(out ILoanSettings loan, ILoanStorageMetadata loanStorageMetadata);


        /// <summary>
        /// Read all rows of ILoanStorageMetadata from repository.
        /// </summary>
        /// <param name="loanStorageMetadataList"></param>
        void Read(out IEnumerable<ILoanStorageMetadata> loanStorageMetadataList);


    }
}
