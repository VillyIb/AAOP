using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eu.iamia.AAOP.CalculationEngine.CalculationV1;
using eu.iamia.AAOP.CalculationEngine.Models;
using eu.iamia.AAOP.ExcelExport;
using eu.iamia.AAOP.ISP.Models;
using eu.iamia.AAOP.ISP.Services;
using eu.iamia.AAOP.RepositoryFile.FileRepository;

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

        private WebExportToExcel zXpExportService;

        private WebExportToExcel XpExportService
        {
            get
            {
                if (null != zXpExportService)
                {
                    return zXpExportService;
                }

                zXpExportService = new WebExportToExcel();
                return zXpExportService;
            }
        }

        private IRepositoryService zXpRepositoryService;

        private IRepositoryService XpRepositoryService
        {
            get
            {
                if (null != zXpRepositoryService)
                {
                    return zXpRepositoryService;
                }

                zXpRepositoryService = new FileRepositoryService();
                return zXpRepositoryService;
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

            XmShowArchive();
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
                XpLoanSettings.PålydendeRente = t1 / 100m;
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
                    Lånebeøb = 50000m, PålydendeRente = 0.08m, Startomkostning = 500m, Løbetid = 5 * 12
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

        protected void XuExport_Click(object sender, EventArgs e)
        {
            XmReadGui();
            XpCalculationService.Init(XpLoanSettings);
            XpCalculationService.CalculateLoan();
            XpLoanCalculations = XpCalculationService.LoanCalculations;


            var outputStream = new StreamWriter(Response.OutputStream);

            try
            {
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();

                Response.AddHeader("content-disposition"
                    , string.Format(
                        "attachment; filename={0}.xml"
                        , @"Betalingsforløb"
                    ));
                Response.AddHeader("Pragma", "public");
                Response.ContentType = "application/vnd.ms-excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;

                XpExportService.Init();
                XpExportService.Convert(outputStream, XpCalculationService.PaymentPlan.PaymentDetailList);

                Response.End();
            }

            catch (Exception)
            {
                // no action
            }

        }

        private void XmShowArchive()
        {
            IEnumerable<ILoanStorageMetadata> archiveList;
            XpRepositoryService.Read(out archiveList);

            var xx = new List<ILoanStorageMetadata>();
            xx.Add(new LoanStorageMetadata
                {
                    Name = @"Vælg lån nedenfor"
                }
            );
            xx.AddRange(archiveList); 

            XuArchiveList.DataSource = xx;
            XuArchiveList.DataTextField = "Name";
            XuArchiveList.DataMember = "Id";
            XuArchiveList.DataBind();
        }

        protected void XuSave_Click(object sender, EventArgs e)
        {
            XmReadGui();

            if (String.IsNullOrWhiteSpace(XuFilename.Text))
            {
                XuFilename.Text = "Filename";
            }

            ILoanStorageMetadata lsm;
            var loanStorage = new LoanStorage
            {
                LoanSettings = XpLoanSettings, LoanStorageMetadata = new LoanStorageMetadata
                {
                    Name = XuFilename.Text
                }
            };

            XpRepositoryService.Create(out lsm, loanStorage);

            XmShowArchive();
        }


        protected void XuArchiveList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = XuArchiveList.SelectedValue;

            IEnumerable<ILoanStorageMetadata> archiveList;
            XpRepositoryService.Read(out archiveList);

            var x = archiveList.FirstOrDefault(t => name.Equals(t.Name, StringComparison.InvariantCultureIgnoreCase));
            if (null != x)
            {
                XuFilename.Text = x.Name;

                ILoanSettings loan;
                XpRepositoryService.Read(out loan, x);

                XpLoanSettings = loan;
                XmFillGui();
                XuCalculate_Click(sender, e);
            }
        }
    }
}