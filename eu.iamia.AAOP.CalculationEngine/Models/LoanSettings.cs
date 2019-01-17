using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eu.iamia.AAOP.CalculationEngine.Models
{
    using eu.iamia.AAOP.ISP.Models;
    public class LoanSettings : ILoanSettings
    {
        public decimal Lånebeøb { get; set; }


        public int Løbetid { get; set; }


        public decimal PålydendeRente { get; set; }


        public decimal Startomkostning { get; set; }


    }
}
