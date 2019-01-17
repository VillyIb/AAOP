using System;

using eu.iamia.AAOP.ISP.Models;

using eu.iamia.AAOP.ISP.Services;

namespace eu.iamia.AAOP.CalculationEngine.CalculationV1
{
    using eu.iamia.AAOP.CalculationEngine.Models;

    public class CalculationServiceV1 : IAAOPCalculationService
    {

        public ILoanSettings LoanSettings { get; protected set; }


        public ILoanCalculations LoanCalculations { get; protected set; }


        public IPaymentPlan PaymentPlan { get; protected set; }


        public void Init(ILoanSettings loanSettings)
        {
            LoanSettings = loanSettings;
            LoanCalculations = new LoanCalculations();
        }


        protected virtual void CalculateYdelse()
        {

            // see: http://www.laaneberegner.nu/beregner.asp
            // see: https://teamtreehouse.com/community/loan-payment-formula-in-c
            // var paymentAmount = (rateOfInterest * loanAmount) / (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));

            var rateOfInterest = (double)LoanSettings.PålydendeRente/12d;
            var loanAmount = (double)LoanSettings.Lånebeøb;
            var numberOfPayments = LoanSettings.Løbetid;

            var paymentAmount  =  (rateOfInterest * loanAmount) / (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));

            Console.WriteLine("RateOfInterest: {0}", rateOfInterest);
            Console.WriteLine("LoanAmount: {0}", loanAmount);
            Console.WriteLine("numberOfPayments: {0}", numberOfPayments);
            Console.WriteLine("paymentAmount: {0}", paymentAmount);

            LoanCalculations.Ydelse = Decimal.Round((decimal)paymentAmount,2);
        }

        protected virtual void CalculateDebitorRente()
        {
            var rateOfInterest = (double)LoanSettings.PålydendeRente / 12d;
            var t1 = 1d + rateOfInterest;
            var t2 = Math.Pow(t1, 12);
            var t3 = t2 - 1d;

            Console.WriteLine("T1 {0}", t1);
            Console.WriteLine("T2 {0}", t2);
            Console.WriteLine("T3 {0}", t3);


            LoanCalculations.DebitorRente = Decimal.Round((decimal)t3,5);
        }


        public virtual void CalculateLoan()
        {

            CalculateYdelse();
            CalculateDebitorRente();
            
        }


    }
}
