using System.Collections.Generic;

namespace eu.iamia.AAOP.ISP.Models
{
    public interface IPaymentDetail
    {
         int Nummer { get; set; }


         decimal PrimoVærdi { get; set; }


         decimal Ydelse { get; set; }


         decimal Afdrag { get; set; }


         decimal Rente { get; set; }


         decimal UltimoVærdi { get; set; }


    }



    public interface IPaymentPlan
    {
        IEnumerable<IPaymentDetail> PaymentDetailList { get; }
    }
}
