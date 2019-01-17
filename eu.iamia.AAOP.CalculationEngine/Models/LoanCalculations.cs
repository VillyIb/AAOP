using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eu.iamia.AAOP.CalculationEngine.Models
{
    using eu.iamia.AAOP.ISP.Models;
    public class LoanCalculations : ILoanCalculations
    {
        public decimal ÅOP { get; set; }


        public decimal DebitorRente { get; set; } 


        public decimal Ydelse { get; set; }


    }
}
