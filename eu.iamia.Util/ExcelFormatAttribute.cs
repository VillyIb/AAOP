namespace eu.iamia.Util
{
    using System;

    /// <summary>
    /// Controls the header naming, order and formatting of columns when exporting class to an Excel file.
    /// Inspired by XmlElementAttribute (Order) and DisplayFormatAttribute.
    /// </summary>
    // ReSharper disable once RedundantAttributeUsageProperty
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ExcelFormatAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the explicit order in which the elements are exported.
        /// </summary>
        public int Order { get; set; }


        /// <summary>
        /// Gets or sets the display format for the field value.
        /// </summary>
        public string DataFormatString { get; set; }


        /// <summary>
        /// Gets or sets the name of the generated XML element.
        /// 
        /// </summary>
        public string ElementName { get; set; }


        /// <summary>
        /// Gets or sets a value that indicates whether the field should be HTML-encoded.
        /// </summary>
        public bool HtmlEncode { get; set; }


        /// <summary>
        /// Gets or sets a value that indicates whether the field should be included in exported row.
        /// </summary>
        public bool IsHidden { get; set; }


        /// <summary>
        /// If provided only include on the specified websites, syntax "WebsiteName,Websitename"
        /// WebsiteName is from [Shared1].[dbo].[WebSites].[siteName]
        /// </summary>
        public string IncludeOnWebsite { get; set; }


        /// <summary>
        /// Gets or or sets the hyperlink to overload on output.
        /// Ignored if blank.
        /// Hyperlink: only specify LocalPath (is without host).
        /// Example: "Pages/Admin/SearchAwb.aspx?awb={0}"
        /// Example: "Pages/All/Link.aspx?Base64=base64-encoded-text
        /// </summary>
        public string Hyperlink { get; set; }

        /// <summary>
        /// Gets ot ses the format of the Base64 encoded part of Hyperlink.
        /// Example: "ForwarderSelectCustomer;{0}" parameter is Id of CustomerPickup row
        /// Example: "CustomerPickupBasket;{0}" parameter is Id of ForwarderPickup row
        /// Example: "ForwarderPickupBasket;{0}" parmeter is Id of ForwarderPickup row
        /// </summary>
        public string Base64Format{ get; set; }


        public ExcelFormatAttribute(string elementName)
        {
            this.ElementName = elementName;
        }

    }
}
