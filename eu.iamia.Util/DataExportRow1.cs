using System;
using nu.gtx.POCO.Contract.Pickup.Constants;

// ReSharper disable InconsistentNaming

namespace nu.gtx.Business.Pickup.Gui.DataExport
{
    /// <summary>
    /// This class defineds the content and column headers for an Excel File.
    /// </summary>
    public class DataExportRow1
    {
        // http://77.66.63.159/Pages/All/Link.aspx?P=B&N=7916

        // Notice: redirection is hardcoded in Link.aspx and the target pages must be able to use the parameter provided.

        // Secret syntax for hyperlink with 2 step access to
        // P=A: http://newdesign.gtx.nu/Pages/Admin/SearchAwb.aspx?awb=7958593005
        // N=awbnumber
        // Host is provided directly to Converter
        // http://77.66.63.158/Pages/All/Link.aspx?P=A&N={some-awb-number}


        /// <summary>
        /// Internal reference number, [Shared1].[Pickup].[CustomerPickup].[Id]
        /// </summary>
        [ExcelFormat("PickupId", Hyperlink = "Pages/All/Link.aspx?P=B&N={0}")] // links to page: http://77.66.63.159/Pages/Pickup/ForwarderV2.aspx?ForwarderPickupId=8566
        public int PickupId { get; set; }


        /// <summary>
        /// First Waybillnumber in Pickup.
        /// </summary>
        [ExcelFormat("Fragtbrevsnummer", Hyperlink = "Pages/All/Link.aspx?P=A&N={0}")] // Links to: http://77.66.63.159/Pages/Admin/SearchAwb.aspx?awb=9674588545
        public string Waybillnumber { get; set; }


        /// <summary>
        /// [Shared1].[Pickup].[CustomerPickup].[PickupDate]
        /// </summary>
        [ExcelFormat("Dato")]
        public DateTime PickupDate { get; set; }


        /// <summary>
        /// [Shared1].[Pickup].[CustomerPickup].[DebitorAccount]
        /// </summary>
        [ExcelFormat("Kunde konto nr.")]
        public string CustomerAccountId { get; set; }


        /// <summary>
        /// [CustomerMainName] until first comma.
        /// </summary>
        [ExcelFormat("Betalende kunde navn")]
        public string CustomerNameShort { get; set; }


        /// <summary>
        /// [CustomerMainName]
        /// </summary>
        [ExcelFormat("Betalende kunde adresse")]
        public string CustomerName { get; set; }


        [ExcelFormat("Afhentningskunde")]
        public string Address_Name { get; set; }


        /// <summary>
        /// True when the address is different form any Account-address.
        /// [IsSpecialAddress]
        /// </summary>
        [ExcelFormat("Fremmed adresse")]
        public bool IsSpecialAddress { get; set; }


        /// <summary>
        /// Concatenated from CustomerPickup.PickupReference and CustomerPickup.OperatorFeedback.
        /// </summary>
        [ExcelFormat("Afhentningsreference")]
        public string PickupReference { get; set; }


        [ExcelFormat("Postnummer")]
        public string Address_Zip { get; set; }


        /// <summary>
        /// Count of [Shared1].[Pickup].[Shipment]
        /// </summary>
        [ExcelFormat("Antal forsendelser")]
        public int CountShipments { get; set; }


        /// <summary>
        /// [TotalWeight]
        /// </summary>
        [ExcelFormat("Sum af kilo")]
        public decimal WeightTotal { get; set; }


        /// <summary>
        /// Count of [Shared1].[Pickup].[ParcelDetails]
        /// </summary>
        [ExcelFormat("Antal kolli")]
        public int CountParcels { get; set; }


        /// <summary>
        /// [PickupOperator]
        /// </summary>
        [ExcelFormat("Leverandør")]
        public string PickupOperator { get; set; }


        /// <summary>
        /// True when a special Sales Price is agreed upon with the Customer.
        /// </summary>
        [ExcelFormat("Specialaftale")]
        public bool IsAgreedPrice { get; set; }


        /// <summary>
        /// What kind of price is agreed upon.
        /// </summary>
        [ExcelFormat("Priskode")]
        public PriceCode PriceCode { get; set; }


        /// <summary>
        /// Our Purchase Price, Optional depending on IsAgreedPrice.
        /// </summary>
        [ExcelFormat("Indkøbspris", DataFormatString = "{0:F2}")]
        public decimal PricePurchase { get; set; }


        /// <summary>
        /// Our Sales Price, Optional depending on IsAgreedPrice.
        /// </summary>
        [ExcelFormat("Salgspris", DataFormatString = "{0:F2}")]
        public decimal PriceSelling { get; set; }


        /// <summary>
        /// Note,  Optional depending on IsAgreedPrice.
        /// </summary>
        [ExcelFormat("Bemærkning CS")]
        public string Note { get; set; }


        /// <summary>
        /// The status of the CustomerPickup.
        /// </summary>
        [ExcelFormat("Pickup status")]
        public PickupStatusV2 PickupStatus { get; set; }


        /// <summary>
        /// Name of website where Customer belongs, Optional - only provided when CustomerWebsite is different from Current Website.
        /// [Shared1].[dbo].[WebSites].[siteName]
        /// </summary>
        [ExcelFormat("Website kunde", IncludeOnWebsite = "GTX")]
        public string CustomerWebsiteName { get; set; }


        /// <summary>
        /// Service Provider Debitor AccountId on , Optional - only provided when CustomerWebsite is different from Current Website.
        /// [Maint].[dbo].[aspnet_CompanyAccount].[AccountID] ([Main].[dbo].[ship_Shipments].[ship_Account])
        /// </summary>
        [ExcelFormat("Webkunde konto nr.", IncludeOnWebsite = "GTX")]
        public string ForwarderWebsiteCustomerAccountId { get; set; }


        /// <summary>
        /// [GTXData].[dbo].[aspnet_CompanyDB].[Company_Name] + ...[CompanyID]
        /// </summary>
        [ExcelFormat("Webkunde navn og id", IncludeOnWebsite = "GTX")]
        public string ForwarderWebsiteCustomerNameShort { get; set; }


        /// <summary>
        /// From shipment [GLS_2015].[dbo].[ship_Shipments].[BillingCompanyName]...
        /// </summary>
        [ExcelFormat("Betalende websitekunde navn", IncludeOnWebsite = "GTX")]
        public string ForwarderWebsiteCustomerName { get; set; }



    }
}
