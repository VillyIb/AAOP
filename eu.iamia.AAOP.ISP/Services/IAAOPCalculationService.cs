namespace eu.iamia.AAOP.ISP.Services
{
    using eu.iamia.AAOP.ISP.Models;

    /// <summary>
    /// Contract required by Loan Calculation Service.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public interface IAAOPCalculationService
    {
        /// <summary>
        /// Initialize Calculator with LoanSettings.
        /// </summary>
        /// <param name="loanSettings"></param>
        void Init(ILoanSettings loanSettings);


        /// <summary>
        /// Calculate Loan.
        /// Required
        /// - LoanSettings
        /// Updates:
        /// - LoanCalculations.
        /// - PaymentPlan 
        /// </summary>
        void CalculateLoan();


        /// <summary>
        /// LoanSettings injected by Init.
        /// </summary>
        ILoanSettings LoanSettings { get; }


       /// <summary>
       /// Base Loan information available after the Loan is Calculated.
       /// </summary>
        ILoanCalculations LoanCalculations { get; }


        /// <summary>
        /// Payment plan available after Loan is Calculated.
        /// </summary>
        IPaymentPlan PaymentPlan { get; }


    }
}
