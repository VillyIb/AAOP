namespace eu.iamia.AAOP.ISP.Models
{
    /// <summary>
    /// Input provided by frontend to AAOP engine.
    /// </summary>
    public interface ILoanSettings
    {
        /// <summary>
        /// 
        /// </summary>
        decimal Lånebeøb { get; set; }

        /// <summary>
        /// Duration in months for loan.
        /// </summary>
        int Løbetid { get; set; }


        /// <summary>
        /// 
        /// </summary>
        decimal PålydendeRente { get; set; }


        /// <summary>
        /// Initial cost to esablish loan.
        /// </summary>
        decimal Startomkostning { get; set; }


    }
}
