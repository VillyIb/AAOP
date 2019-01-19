using System;

using eu.iamia.AAOP.ISP.Models;

using eu.iamia.AAOP.ISP.Services;
// ReSharper disable InconsistentNaming

namespace eu.iamia.AAOP.CalculationEngine.CalculationV1
{
    using eu.iamia.AAOP.CalculationEngine.Models;

    using Microsoft.VisualBasic;

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


        // Check calculations
        // see: http://www.laaneberegner.nu/beregner.asp


        protected internal virtual void CalculateYdelse()
        {
            // Loan Payment formula in C#
            // see: https://teamtreehouse.com/community/loan-payment-formula-in-c

            // var paymentAmount = (rateOfInterest * loanAmount) / (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));

            var rateOfInterest = (double)LoanSettings.PålydendeRente / 12d;
            var loanAmount = (double)LoanSettings.Lånebeøb;
            var numberOfPayments = LoanSettings.Løbetid;

            var paymentAmount = (rateOfInterest * loanAmount) / (1 - Math.Pow(1 + rateOfInterest, numberOfPayments * -1));

            Console.WriteLine("RateOfInterest: {0}", rateOfInterest);
            Console.WriteLine("LoanAmount: {0}", loanAmount);
            Console.WriteLine("numberOfPayments: {0}", numberOfPayments);
            Console.WriteLine("paymentAmount: {0}", paymentAmount);

            LoanCalculations.Ydelse = Decimal.Round((decimal)paymentAmount, 2);
        }

        protected internal virtual void CalculateDebitorRente()
        {
            // effektiv rente formel
            // see: https://www.studieportalen.dk/kompendier/matematik/formelsamling/rentesregning/rente/effektiv-rente

            var rateOfInterest = (double)LoanSettings.PålydendeRente / 12d;
            var t1 = 1d + rateOfInterest;
            var t2 = Math.Pow(t1, 12);
            var t3 = t2 - 1d;

            Console.WriteLine("T1 {0}", t1);
            Console.WriteLine("T2 {0}", t2);
            Console.WriteLine("T3 {0}", t3);


            LoanCalculations.DebitorRente = Decimal.Round((decimal)t3, 5);
        }


        protected internal int RateCalculationPrecision { get; set; }




        /*
         * 
         *   apr = ((loanamount + extracost) * rate * Math.Pow((1 + rate), duration)) / (Math.Pow((1 + rate),duration) - 1);
         * */

        protected internal virtual void ZCalculateAAOP()
        {
            // see:https://ungdomsbyen.dk/wp-content/uploads/2017/03/udregning-af-acc8aop-1.pdf

            var bruttoPayment = LoanSettings.Lånebeøb + LoanSettings.Startomkostning;

            var years = LoanSettings.Løbetid / 12;

            var aaop = LoanSettings.Startomkostning / LoanSettings.Løbetid / years - 1m;

            LoanCalculations.ÅOP = decimal.Round(aaop, 5);

        }


        protected internal double Rente(double hovedstol, double afdrag, int perioder)
        {
            var error = Math.Pow(10, -5);
            var start = 0.007d;

            Console.WriteLine("Hovedstol: {0}, Afdrag: {1}, Perioder: {2}", hovedstol, afdrag, perioder);
            Console.WriteLine("Start {0}, Error: {1}", error, start);

            double value = start;
            for (var loop = 100; loop >= 0; loop--)
            {
                value = start;
                var beregnetAfdrag = -hovedstol * value / (1 - Math.Pow(1 + value, -perioder));
                var afvigelse = afdrag - beregnetAfdrag;
                Console.WriteLine("Rente: {0}, Afvigelse: {1}, Afdrag:{2}", value, afvigelse, beregnetAfdrag);
                if (error > Math.Abs(afvigelse))
                {
                    break;
                }

                start = start - afvigelse *.00001;

            }

            return value;

        }


        // https://erhv-oekon-akademi.hansreitzel.dk/elementer/dokument/skabelon-tiler-beregning-af-aaop-og-amortiseringstabel/model-aaop-annuitetslaan-flere-terminer.aspx
        // http://www.laaneberegner.nu/beregner.asp

        protected internal virtual void CalculateAAOP()
        {
            var nper = (double)LoanSettings.Løbetid;
            var pmt = -(double)LoanCalculations.Ydelse;
            var pv = (double)(LoanSettings.Lånebeøb - LoanSettings.Startomkostning );

            Console.WriteLine("Nper:{0} Pmt:{1} Pv:{2} ",nper, pmt, pv);

            var rate = Microsoft.VisualBasic.Financial.Rate(nper, pmt, pv);
            Console.WriteLine("MonthlyRate: {0}", rate);

            var apr = Math.Pow(1 + rate, 12) - 1;

            LoanCalculations.ÅOP = Decimal.Round((decimal)apr, 5);

        }


        public virtual void CalculateLoan()
        {

            CalculateYdelse();
            CalculateDebitorRente();
            this.CalculateAAOP();
        }


    }
}
