using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using EU.Iamia.Logging;
using nu.gtx.CodeFirst.Model.Pickup;
using nu.gtx.CodeFirst.Model.PickupDataExport;
using nu.gtx.Common1.Exceptions;
using nu.gtx.Common1.Extensions;
using nu.gtx.Common1.Utils;
using nu.gtx.POCO.Contract.Pickup.Constants;

namespace nu.gtx.Business.Pickup.Gui.DataExport
{
    public class DataExportService : BaseService, IDataExportService
    {
        #region Overrides of ControllerBase

        private static readonly ILog LoggerStatic = LogManager.GetLogger();
        protected override ILog Logger { get { return LoggerStatic; } }

        #endregion


        private const string ExportName = "PickupAccounting";


        private readonly string CurrentWebsiteName;


        #region P: private ExcelConverterV2<DataExportRow1> Converter { get; }
        private ExcelConverterV2<DataExportRow1> zConverter;

        private ExcelConverterV2<DataExportRow1> Converter
        {
            get { return zConverter ?? (zConverter = new ExcelConverterV2<DataExportRow1>(CurrentWebsiteName)); }
        }
        #endregion


        /// <summary>
        /// Calculates the ExportRow for each selected CustomerPickup, result is saved in PickupAccountingRow.ExportData.
        /// </summary>
        /// <param name="exportHeader"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public int CalculatePickupAccountingRow(ExportHeader exportHeader, Uri host)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                var pickupAccountingRows = ContextSharedPickup.PickupAccountingRow.Where(t => t.Fk_ExportHeader_Id == exportHeader.Id).ToList();

                foreach (var pickupAccountingRow in pickupAccountingRows)
                {
                    pickupAccountingRow.ExportData = String.Empty;
                    pickupAccountingRow.Sorting = String.Empty;
                    ContextSharedPickup.SaveChanges();
                }

                foreach (var pickupAccountingRow in pickupAccountingRows)
                {
                    var customerPickup = ContextSharedPickup.CustomerPickup.Include(t => t.ForwarderPickup).FirstOrDefault(
                        t =>
                            t.Id == pickupAccountingRow.FK_CustomerPickup_Id
                        );

                    if (customerPickup == null) { continue; }
                    if (customerPickup.ForwarderPickup == null)
                    {
                        Logger.ErrorFormat("Orphan CustomerPickup.Id: {0}", customerPickup.Id);
                        continue;
                    }

                    DataExportRow1 exportRow;
                    if (Convert(out exportRow, customerPickup))
                    {
                        var row = Converter.ConvertRow(exportRow, host);
                        pickupAccountingRow.ExportData = row;
                        pickupAccountingRow.Sorting = String.Format(
                            "{0:yyyy-MM-dd};{1,8};{2,8};{3,8},{4,8}"
                            , customerPickup.PickupDate
                            , customerPickup.FK_Customer_Id
                            , customerPickup.DebitorAccount
                            , customerPickup.Address.Zip
                            , customerPickup.Id
                        );
                    }

                    ContextSharedPickup.SaveChanges();
                }

                exportHeader.HistoryLog = "; ".Concatenate(exportHeader.HistoryLog, String.Format("{0:yyyy-MM-dd HH:mm} Calculated", SystemDateTime.Now));

                if (exportHeader.ExportStatus == ExportStatus.Requested)
                {
                    exportHeader.ExportStatus = ExportStatus.Calculated;
                }

                ContextSharedPickup.SaveChanges();

                return 0;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Returns true if the ExportHeader specified by batchNo can be deleted.
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool CanDelete(int batchNo)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                ExportHeader exportHeader;
                if (Read(out exportHeader, batchNo))
                {
                    var canDelete =
                        (exportHeader.ExportStatus == ExportStatus.Requested
                         || exportHeader.ExportStatus == ExportStatus.Calculated
                            )
                        && exportHeader.TimestampCreate >= SystemDateTime.Yesterday;

                    canDelete |=
                        exportHeader.ExportStatus == ExportStatus.Hidden
                        &&
                        exportHeader.TimestampCreate <= SystemDateTime.Today.AddMonths(-3);

                    return canDelete;
                }

                return false;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Returns true if the ExportHeader specified by batchNo is Locked and the row is more than 3 months old.
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool CanHide(int batchNo)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                ExportHeader exportHeader;
                if (Read(out exportHeader, batchNo))
                {
                    var canLock = exportHeader.ExportStatus == ExportStatus.Locked
                    &&
                    exportHeader.TimestampCreate < SystemDateTime.Today.AddMonths(-3);

                    return canLock;
                }

                return false;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Returns true if the ExportHeader specified by batchNo has an ExportStatus of Calculated and is more than 3 monts old.
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool CanLock(int batchNo)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                ExportHeader exportHeader;
                if (Read(out exportHeader, batchNo))
                {
                    var canLock = exportHeader.ExportStatus == ExportStatus.Calculated;

                    return canLock;
                }

                return false;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Returns true if ExportHeader with the specified BatchNo is Requested or Calculated.
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool CanRecalculate(int batchNo)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                ExportHeader exportHeader;
                if (Read(out exportHeader, batchNo))
                {
                    var canRecalculate =
                        exportHeader.ExportStatus == ExportStatus.Requested
                        || exportHeader.ExportStatus == ExportStatus.Calculated;

                    return canRecalculate;
                }

                return false;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        private bool ChangeStatus(int batchNo, ExportStatus newStatus)
        {
            ExportHeader exportHeader;
            if (Read(out exportHeader, batchNo))
            {
                var before = exportHeader.ExportStatus;
                exportHeader.ExportStatus = newStatus;
                var msg = String.Format("{0:yyyy-MM-dd HH:mm}, {1:G} -> {2:G}", SystemDateTime.Now, before, newStatus);
                exportHeader.HistoryLog = "; ".Concatenate(exportHeader.HistoryLog, msg);
                ContextSharedPickup.SaveChanges();
                return true;
            }

            return false;
        }


        private bool IsGtxWebsiteRequestor
        {
            get { return "GTX".Equals(CurrentWebsiteName, StringComparison.OrdinalIgnoreCase); }
        }


        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool Convert(out DataExportRow1 value, CustomerPickup customer)
        {
            // Customer(Pickup) references parent ForwarderPickp
            // If a value isn't provided (0/default value) on the CustomerPickup 
            // is the same value on the parent ForwarderPickup used.

            Logger.DebugFormat("+Enter");
            try
            {
                var addressName = "DK".Equals(customer.Address.CountryCode, StringComparison.OrdinalIgnoreCase)
                    ? ""
                    : String.Format("*{0} ,", customer.Address.CountryCode);

                addressName += String.Format(
                    "{0}, {1}, {2}, {3}"
                    , customer.Address.Name
                    , customer.Address.Street1
                    , customer.Address.Zip
                    , customer.Address.City
                ).Truncate(50);

                var customerWebsiteName = DbSharedStandard.WebSites.First(t => t.WebsiteID.Equals(customer.WebsiteId)).siteName;

                var firstCommaPosition = customer.CustomerMainName.IndexOf(',', 0);
                var customerNameShort = firstCommaPosition > 2
                    ? customer.CustomerMainName.Substring(0, firstCommaPosition)
                    : "";


                var countParcels = ContextSharedPickup.ParcelDetails.Count(
                    t =>
                    t.Shipment.CustomerPickupId == customer.Id
                );

                var countShipments = ContextSharedPickup.Shipment.Count(
                    t =>
                    t.CustomerPickupId == customer.Id
                );

                var priceCode = customer.PriceCode != PriceCode.Standardpris
                    ? customer.PriceCode
                    : customer.ForwarderPickup.PriceCode
                    ;

                var isAgreedPrice = priceCode != PriceCode.Standardpris;  // more specific rule to apply.

                var note = customer.ForwarderPickup.NoteInternal;

                var pickupReference = String.IsNullOrWhiteSpace(customer.PickupReference)
                    ? customer.ForwarderPickup.PickupReference
                    : customer.PickupReference
                    ;

                pickupReference = "; ".Concatenate(
                    pickupReference
                    , String.IsNullOrWhiteSpace(customer.OperatorFeedback)
                    ? customer.ForwarderPickup.OperatorFeedback
                    : customer.OperatorFeedback
                    );

                var pricePurchase = isAgreedPrice
                    ? (customer.PricePurchase >= 0.01m
                        ? customer.PricePurchase
                        : customer.ForwarderPickup.PricePurchase)
                    : 0m;

                var priceSelling = isAgreedPrice
                    ? (customer.PriceSelling >= 0.01m
                        ? customer.PriceSelling
                        : customer.ForwarderPickup.PriceSelling)
                    : 0m;


                var pickupOperator = ContextSharedPickup.ForwarderPickup.First(t => t.Id == customer.FK_ForwarderPickup_Id).PickupOperatorString;


                var forwarderWebsiteCustomerName = "na";
                var forwarderWebsiteCustomerAccountId = "na";
                var forwarderWebsiteCustomerNameShort = "na";

                var shipment =
                    ContextSharedPickup.Shipment.Where(t => t.CustomerPickupId == customer.Id)
                        .OrderBy(t => t.Id)
                        .FirstOrDefault();


                if (shipment != null)
                {
                    var fwShipment = DbMainStandard.ship_Shipments.FirstOrDefault(
                        t =>
                        t.ship_AWB == shipment.WaybillNumber
                        &&
                        t.ship_date == shipment.ShipmentDate
                        );

                    if (fwShipment != null)
                    {
                        forwarderWebsiteCustomerAccountId = IsGtxWebsiteRequestor
                            ? "100000".Substring(0, 6 - fwShipment.ship_Account.Length) + fwShipment.ship_Account
                            : fwShipment.ship_Account
                        ;

                        var accountId = int.Parse(fwShipment.ship_Account);

                        var companyAccount = DbMainStandard.aspnet_CompanyAccount.First(t => t.AccountID == accountId);

                        var company = DbMainStandard.aspnet_CompanyDB.First(t => t.CompanyID == companyAccount.CompanyID);

                        forwarderWebsiteCustomerNameShort = String.Format(
                            "{0} ({1})"
                            , company.Company_Name
                            , company.CompanyID
                            );

                        forwarderWebsiteCustomerName = String.Format(
                            "{0}, {1}, {2}, {3}, "
                            , companyAccount.BillingCompanyName
                            , companyAccount.BillingAddress1
                            , companyAccount.BillingZip
                            , companyAccount.BillingCity
                            );
                    }
                }

                value = new DataExportRow1
                {
                    CustomerAccountId = customer.DebitorAccount,
                    Address_Name = addressName,
                    Address_Zip = customer.Address.Zip,
                    CustomerWebsiteName = customerWebsiteName,
                    CountParcels = countParcels,
                    CountShipments = countShipments,
                    CustomerNameShort = customerNameShort,
                    CustomerName = customer.CustomerMainName,
                    IsAgreedPrice = isAgreedPrice,
                    IsSpecialAddress = customer.IsSpecialAddress,
                    Note = note,
                    PickupDate = customer.PickupDate,
                    PickupId = customer.Id,
                    PickupOperator = pickupOperator,
                    PickupReference = pickupReference,
                    PickupStatus = customer.PickupStatus,
                    PriceCode = priceCode,
                    PricePurchase = pricePurchase,
                    PriceSelling = priceSelling,
                    ForwarderWebsiteCustomerAccountId = forwarderWebsiteCustomerAccountId,
                    ForwarderWebsiteCustomerNameShort = forwarderWebsiteCustomerNameShort,
                    ForwarderWebsiteCustomerName = forwarderWebsiteCustomerName,
                    Waybillnumber = shipment != null ? shipment.WaybillNumber : "na",
                    WeightTotal = customer.TotalWeight,
                };

                return true;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Outputs report as Header, rows and End.
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="exportHeader"></param>
        /// <returns></returns>
        public void Convert(StreamWriter outputStream, ExportHeader exportHeader)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                var header = Converter.ConvertHeader("Pickup Accounting Information", "Pickup Export");
                outputStream.WriteLine(header);

                var pickupAccountingRows = ContextSharedPickup.PickupAccountingRow.Where(
                    t =>
                    t.Fk_ExportHeader_Id == exportHeader.Id
                ).OrderBy(t => t.Sorting);

                foreach (var pickupAccountingRow in pickupAccountingRows)
                {
                    outputStream.WriteLine(pickupAccountingRow.ExportData);     // TODO rows should be aggregated before output and consolidated with PickupFee.
                }

                var tail = Converter.ConvertEnd();
                outputStream.WriteLine(tail);
                outputStream.Flush();
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool Create(out ExportHeader value, string historyLog, string requestor)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                var websiteId = DbMainStandard.CurrentWebsite.First().ApplicationId;

                var batchNo =
                    ContextSharedPickup.ExportHeader.Any(
                        t => t.Name.Equals(ExportName, StringComparison.OrdinalIgnoreCase))
                        ? ContextSharedPickup.ExportHeader.Where(
                            t => t.Name.Equals(ExportName, StringComparison.OrdinalIgnoreCase)).Max(t => t.BatchNo) + 1
                        : 1;

                value = ContextSharedPickup.ExportHeader.Add(new ExportHeader
                {
                    BatchNo = batchNo,
                    ExportStatus = ExportStatus.Requested,
                    HistoryLog = historyLog,
                    Name = ExportName,
                    Requestor = requestor,
                    RowCount = 0,
                    TimestampCreate = SystemDateTime.UtcNow,
                    WebsiteId = websiteId,
                    WebsiteIdHash = websiteId.GetHashCode()
                });
                ContextSharedPickup.SaveChanges();

                return true;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Deletes the ExportHeader with the specified BatchNo and its Children.
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool Delete(int batchNo)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                if (!(CanDelete(batchNo))) { return false; }

                ExportHeader exportHeader;
                if (Read(out exportHeader, batchNo))
                {
                    var list = ContextSharedPickup.PickupAccountingRow.Where(t => t.Fk_ExportHeader_Id == exportHeader.Id).ToList();
                    ContextSharedPickup.PickupAccountingRow.RemoveRange(list);
                    ContextSharedPickup.ExportHeader.Remove(exportHeader);
                    ContextSharedPickup.SaveChanges();
                    return true;
                }

                return false;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Changes the status of the ExportHeader with the specified BatchNo from Locked to Hidden.
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool Hide(int batchNo)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                return CanHide(batchNo) && ChangeStatus(batchNo, ExportStatus.Hidden);
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Changes the status of the ExportHeader with the specified BatchNo from Calculated to Locked
        /// </summary>
        /// <param name="batchNo"></param>
        /// <returns></returns>
        public bool Lock(int batchNo)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                return CanLock(batchNo) && ChangeStatus(batchNo, ExportStatus.Locked);
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        public int Read(out List<ExportHeader> exportHeaderList, bool includeHidden, int maxRows)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                var filterHidden = ExportStatus.Hidden.ToString("G");

                var list1 = includeHidden
                    ?
                    ContextSharedPickup.ExportHeader.Where(
                        t =>
                        t.Name.Equals(ExportName, StringComparison.OrdinalIgnoreCase)
                    )
                    : ContextSharedPickup.ExportHeader.Where(
                        t =>
                        t.Name.Equals(ExportName, StringComparison.OrdinalIgnoreCase)
                        &&
                        !(filterHidden.Equals(t.ExportStatusString, StringComparison.OrdinalIgnoreCase))
                    ).Take(maxRows) // maxRows is only active when not showing Hidden
                    ;

                exportHeaderList = list1.ToList();
                return exportHeaderList.Count;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        public bool Read(out ExportHeader value, int batchNo)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                value = ContextSharedPickup.ExportHeader.FirstOrDefault(t => t.BatchNo == batchNo);
                return value != null;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        /// <summary>
        /// Select data not previously selected on any PickupAccounting report.
        /// </summary>
        /// <returns></returns>
        public int SelectData(out ExportHeader exportHeader, DateTime from, DateTime until, string requestor)
        {
            Logger.DebugFormat("+Enter");
            try
            {
                if (from > until) { throw new GtxClientException("from > until", "XuFromError2", new object[] { "from > until" }); }
                //if (until > SystemDateTime.Yesterday) { throw new GtxClientException("Illegal until date", "XuUntilError2", new object[] { SystemDateTime.Yesterday }); }
                if (until > SystemDateTime.Today) { throw new GtxClientException("Illegal until date", "XuUntilError2", new object[] { SystemDateTime.Today }); } // TODO use yesteday

                Create(out exportHeader, String.Format("From: {0:yyyy-MM-dd} until: {1:yyyy-MM-dd}", from, until), requestor);

                const string sql =
                    "SELECT "
                    + "Pickup.CustomerPickup.Id "
                    + "FROM "
                    + "Pickup.CustomerPickup "
                    + "LEFT OUTER JOIN "
                    + "Pickup.PickupAccountingRow "
                    + "ON Pickup.CustomerPickup.Id = Pickup.PickupAccountingRow.FK_CustomerPickup_Id "
                    + "WHERE "
                    + "{0} <= Pickup.CustomerPickup.PickupDate "
                    + "and "
                    + "Pickup.CustomerPickup.PickupDate <= {1} "
                    + "and "
                    + "Pickup.CustomerPickup.PickupStatus != N'CustHand' "   // TODO maybe som kind of closed or finished
                    + "and "
                    + "Pickup.PickupAccountingRow.Id IS NULL " // not part in any existing report.
                    ;

                var sqlCommand = ContextSharedPickup.Database.SqlQuery<int>(sql, from, until);

                var rowList = sqlCommand.ToList();

                var count = 0;

                foreach (var customerPickupId in rowList)
                {
                    ContextSharedPickup.PickupAccountingRow.Add(new PickupAccountingRow
                    {
                        ExportData = String.Empty,
                        FK_CustomerPickup_Id = customerPickupId,
                        Fk_ExportHeader_Id = exportHeader.Id,
                        Sorting = String.Empty
                    }
                    );
                    count++;
                }

                exportHeader.RowCount = count;

                if (count == 0)
                {
                    ContextSharedPickup.ExportHeader.Remove(exportHeader);
                }

                ContextSharedPickup.SaveChanges();

                return count;
            }

            finally
            {
                Logger.DebugFormat("-Exit");
            }
        }


        public DataExportService(CommonContext commonContext) : base(commonContext)
        {
            var currentWebsiteId = DbMainStandard.CurrentWebsite.First().ApplicationId;

            CurrentWebsiteName = DbSharedStandard.WebSites.First(t => t.WebsiteID == currentWebsiteId).siteName;
        }

    }
}
