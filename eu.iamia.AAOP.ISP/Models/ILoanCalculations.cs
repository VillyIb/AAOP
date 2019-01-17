namespace eu.iamia.AAOP.ISP.Models
{
    public interface ILoanCalculations
    {
        /// <summary>
        /// Årlig Omkostning I Procent
        /// </summary>
        // ReSharper disable once InconsistentNaming
        decimal ÅOP { get; set; }


        /// <summary>
        /// 
        /// </summary>
        decimal DebitorRente { get; set; }


        /// <summary>
        /// Monthly payment.
        /// </summary>
        decimal Ydelse { get; set; }


    }
}
