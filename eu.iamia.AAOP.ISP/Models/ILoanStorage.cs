namespace eu.iamia.AAOP.ISP.Models
{
    /// <summary>
    /// Tag to control Repositor Create and Read of Loan Settings.
    /// </summary>
    public interface ILoanStorage
    {
        ILoanStorageMetadata LoanStorageMetadata { get; set; }


        ILoanSettings LoanSettings { get; set; }
    }
}
