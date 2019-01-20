using System.Collections.Generic;

namespace eu.iamia.AAOP.CalculationEngine.Models
{
    using eu.iamia.AAOP.ISP.Models;
    using eu.iamia.Util;

    public class PaymentDetail : IPaymentDetail
    {
        [ExcelFormat("Termin nr.")]
        public int Nummer { get; set; }


        [ExcelFormat("Primo værdi", DataFormatString = "{0:F2}")]
        public decimal PrimoVærdi { get; set; }


        [ExcelFormat("Ydelse", DataFormatString = "{0:F2}")]
        public decimal Ydelse { get; set; }


        [ExcelFormat("Afdrag", DataFormatString = "{0:F2}")]
        public decimal Afdrag { get; set; }


        [ExcelFormat("Rente beløb", DataFormatString = "{0:F2}")]
        public decimal Rente { get; set; }


        [ExcelFormat("Ultimo værdi", DataFormatString = "{0:F2}")]
        public decimal UltimoVærdi { get; set; }


    }


    public class PaymentPlan : IPaymentPlan
    {
        private List<IPaymentDetail> zPaymentDetailList;

        public List<IPaymentDetail> PaymentDetailList
        {
            get
            {
                return zPaymentDetailList ?? (zPaymentDetailList = new List<IPaymentDetail>());
            }
        }

        IEnumerable<IPaymentDetail> IPaymentPlan.PaymentDetailList => PaymentDetailList;
    }
}
