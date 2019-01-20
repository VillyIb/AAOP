namespace eu.iamia.AAOP.RepositoryFile.FileRepository
{
    using eu.iamia.AAOP.ISP.Models;

    public     class LoanStorage : ILoanStorage 
    {
        public ILoanStorageMetadata LoanStorageMetadata { get; set; }


        public ILoanSettings LoanSettings { get; set; }


    }
}
