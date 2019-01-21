using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eu.iamia.AAOP.CalculationEngine.CalculationV1;
using eu.iamia.AAOP.CalculationEngine.Models;
using eu.iamia.AAOP.ISP.Models;
using eu.iamia.AAOP.ISP.Services;

namespace eu.iamia.AAOP.GUI
{
    public partial class Låneberegning : System.Web.UI.Page
    {

        #region Properties Xp...

        private ILoanSettings XpLoanSettings { get; set; }


        private ILoanCalculations XpLoanCalculations { get; set; }


        private IAAOPCalculationService zCalculationService;

        private IAAOPCalculationService XpCalculationService
        {
            get
            {
                if (null != zCalculationService)
                {
                    return zCalculationService;
                }

                zCalculationService = new CalculationServiceV1();
                return zCalculationService;
            }
        }

        #endregion

        #region Service Methods Xm...

        private void XmFillGui()
        {
            if (null != XpLoanCalculations)
            {
                XuAAOP.Text = (XpLoanCalculations.ÅOP * 100m).ToString("0.00");
                XuDebRente.Text = (XpLoanCalculations.DebitorRente * 100m).ToString("0.00");
                XuYdelse.Text = XpLoanCalculations.Ydelse.ToString("0.00");
            }

            XuHovedstol.Text = XpLoanSettings.Lånebeøb.ToString("0.00");
            XuNumberOfPeriods.Text = XpLoanSettings.Løbetid.ToString("0");
            XuRente.Text = (XpLoanSettings.PålydendeRente * 100m).ToString("0.00");
            XuOmkostning.Text = XpLoanSettings.Startomkostning.ToString("0.00");
        }


        private void XmRead(out decimal value, TextBox control)
        {
            decimal t1;
            value = decimal.TryParse(control.Text, out t1) ? t1 : 0m;
        }

        private void XmRead(out int value, TextBox control)
        {
            int t1;
            value = Int32.TryParse(control.Text, out t1) ? t1 : 0;
        }

        private void XmReadGui()
        {
            XpLoanSettings = new LoanSettings();

            {
                decimal t1;
                XmRead(out t1, XuHovedstol);
                XpLoanSettings.Lånebeøb = t1;
            }

            {
                int t1;
                XmRead(out t1, XuNumberOfPeriods);
                XpLoanSettings.Løbetid = t1;
            }

            {
                decimal t1;
                XmRead(out t1, XuRente);
                XpLoanSettings.PålydendeRente = t1/100m;
            }

            {
                decimal t1;
                XmRead(out t1, XuOmkostning);
                XpLoanSettings.Startomkostning = t1;
            }
        }

        #endregion


        #region System Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (false == IsPostBack)
            {
                // GET
                XpLoanCalculations = null;

                XpLoanSettings = new LoanSettings
                {
                    Lånebeøb = 10000m, PålydendeRente = 0.08m, Startomkostning = 1000m, Løbetid = 5 * 12
                };

                XmFillGui();
            }

            else
            {
                // Post

            }
        }
        #endregion

        protected void XuCalculate_Click(object sender, EventArgs e)
        {
            XmReadGui();
            XpCalculationService.Init(XpLoanSettings);
            XpCalculationService.CalculateLoan();
            XpLoanCalculations = XpCalculationService.LoanCalculations;
            XmFillGui();
        }
    }
}