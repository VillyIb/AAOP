namespace eu.iamia.Util
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using eu.iamia.Util.Extensions;

    /// <summary>
    /// Outputs class or structure to ExcelFile format is Excel 2003 XML.
    /// Usage:
    /// var api = new ExcelConverterV2&lt;T&gt;(StreamWriter);
    /// api.ConvertHeader();
    /// api.ConvertRow(row); * repeat for all rows
    /// api.ConverEnd();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelConverterV2<T> where T : new()
    {
        // ReSharper disable once StaticMemberInGenericType


        private readonly string CurrentWebsiteName;


        protected class PropertyInfoDecoratorV2
        {
            public PropertyInfo PropertyInfo { get; set; }

            public ExcelFormatAttribute ExcelFormatAttribute { get; set; }
        }


        private List<PropertyInfoDecoratorV2> PropertyInfoList { get; set; }


        /// <summary>
        /// Loads the properties for the generic class T and extact custom attribute ExcelFomatAttribute.
        /// </summary>
        public void LoadPropertyInfoList(ICollection<string> propertyOrder)
        {
            if (this.PropertyInfoList.HasValue()) { return; }

            var propertyList = typeof(T).GetProperties();

            var allColumnNames = new List<PropertyInfoDecoratorV2>(propertyList.Length);

            var unassignedOrder = propertyList.Length;
            foreach (var property in propertyList)
            {
                var attributeList = property.GetCustomAttributes(typeof(ExcelFormatAttribute), false);

                var excelFormatAttribute = (attributeList.FirstOrDefault() as ExcelFormatAttribute) ??
                                           new ExcelFormatAttribute(property.Name)
                                           {
                                               DataFormatString = null, // explicit indicate it is undefined.
                                               Order = unassignedOrder
                                           };

                var helper = new PropertyInfoDecoratorV2
                {
                    ExcelFormatAttribute = excelFormatAttribute,
                    PropertyInfo = property,
                };

                allColumnNames.Add(helper);

                unassignedOrder++;
            }

            var filteredByCurrentWebsiteName = allColumnNames.Where(
                pi =>
                String.IsNullOrWhiteSpace(pi.ExcelFormatAttribute.IncludeOnWebsite) // Nothing specified
                ||
                pi.ExcelFormatAttribute.IncludeOnWebsite.IndexOf(this.CurrentWebsiteName, StringComparison.OrdinalIgnoreCase) >= 0  // Current website is specified
            );

            if (propertyOrder.Any())
            {
                var propertyOrderList = propertyOrder.ToList();
                var filteredByPropertyOrder = filteredByCurrentWebsiteName.Where(t1 => propertyOrder.Any(po => t1.PropertyInfo.Name == po)).ToList();

                this.PropertyInfoList = filteredByPropertyOrder.OrderBy(pi => propertyOrderList.IndexOf(pi.PropertyInfo.Name)).ToList();
            }
            else
            {
                this.PropertyInfoList = filteredByCurrentWebsiteName.OrderBy(t => t.ExcelFormatAttribute.Order).ToList();
            }
        }


        public string ConvertHeader(string title, string author, string keywords = null, string worksheetName = null)
        {
            this.LoadPropertyInfoList(new List<string>(0));

            var result = new StringBuilder();

            result.AppendFormat("<?xml version=\"1.0\"?>");
            result.AppendFormat("\r\n<?mso-application progid=\"Excel.Sheet\"?>");
            result.AppendFormat("\r\n<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\" xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
            result.AppendFormat("\r\n <DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
            result.AppendFormat("\r\n  <{0}>{1}</{0}>", "Title", title);
            result.AppendFormat("\r\n  <{0}>{1}</{0}>", "Author", author);
            result.AppendFormat("\r\n  <{0}>{1}</{0}>", "LastAuthor", author);
            result.AppendFormat("\r\n  <{0}>{1}</{0}>", "Keywords", keywords ?? String.Empty);
            result.AppendFormat("\r\n  <{0}>{1:u}</{0}>", "Created", SystemDateTime.Now);
            result.AppendFormat("\r\n  <{0}>{1}</{0}>", "Version", "16.00");
            result.AppendFormat("\r\n </DocumentProperties>");
            result.AppendFormat("\r\n <Styles>");
            result.AppendFormat("\r\n  <Style ss:ID=\"header\" ss:Name=\"Normal\"><Interior ss:Color=\"#d4e5b4\" ss:Pattern=\"Solid\"/></Style>");
            result.AppendFormat("\r\n  <Style ss:ID=\"shortdate\"><Interior/><NumberFormat ss:Format=\"Short Date\"/></Style>");
            result.AppendFormat("\r\n  <Style ss:ID=\"hyperlink\" ss:Name=\"Hyperlink\"><Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#0563C1\" ss:Underline=\"Single\"/></Style>");
            result.AppendFormat("\r\n  <Style ss:ID=\"s66\"><NumberFormat ss:Format=\"Standard\"/></Style>");
            result.AppendFormat("\r\n </Styles>");
            result.AppendFormat("\r\n <Worksheet ss:Name=\"{0}\">", worksheetName ?? "Sheet1");
            result.AppendFormat("\r\n  <Table >");

            result.AppendFormat("\r\n   <Row>");
            {
                foreach (var property in this.PropertyInfoList)
                {
                    result.AppendFormat(
                        "\r\n    <Cell ss:StyleID=\"header\"><Data ss:Type=\"String\">{0}</Data></Cell>"
                        , property.ExcelFormatAttribute.ElementName
                    );
                }
            }
            result.AppendFormat("\r\n   </Row>");

            return result.ToString();
        }


        public string ConvertRow(T row, Uri host)
        {
            this.LoadPropertyInfoList(new List<string>(0));

            var result = new StringBuilder();

            result.AppendFormat("\r\n   <Row>");

            var rowType = typeof(T);

            foreach (var property in this.PropertyInfoList)
            {
                try
                {
                    var cellAttribute = String.Empty;

                    var value = rowType.GetProperty(property.PropertyInfo.Name).GetValue(row, null);

                    if (value != null && !("na".Equals(value.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        if (property.ExcelFormatAttribute.Hyperlink.HasValue())
                        {
                            var t1 = value;

                            if (property.ExcelFormatAttribute.Hyperlink.IndexOf("Base64", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                var raw = String.Format(property.ExcelFormatAttribute.Base64Format, value);
                                t1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
                            }

                            var page = String.Format(property.ExcelFormatAttribute.Hyperlink, t1);

                            cellAttribute = String.Format(
                                " ss:StyleID=\"hyperlink\" ss:HRef=\"{0}\"",
                                new Uri(host, page)
                            );
                        }
                    }

                    if (property.PropertyInfo.PropertyType.IsEnumType())
                    {
                        result.AppendFormat(
                            "\r\n    <Cell{1}><Data ss:Type=\"String\">{0:G}</Data></Cell>"
                            , value
                            , cellAttribute
                        );
                    }

                    else if (property.PropertyInfo.PropertyType.IsNummericType())
                    {
                        cellAttribute = " ss:StyleID=\"s66\"";
                        var format = String.IsNullOrWhiteSpace(property.ExcelFormatAttribute.DataFormatString)
                            ? "{0}"
                            : property.ExcelFormatAttribute.DataFormatString;

                        var valueNummeric = String.Format(
                            CultureInfo.InvariantCulture
                            , format
                            , value
                            );

                        result.AppendFormat(
                            "\r\n    <Cell{1}><Data ss:Type=\"Number\">{0}</Data></Cell>"
                            , valueNummeric
                            , cellAttribute
                        );
                    }

                    else if (property.PropertyInfo.PropertyType.IsDateTime())
                    {
                        var format = String.IsNullOrWhiteSpace(property.ExcelFormatAttribute.DataFormatString)
                            ? "{0:yyyy-MM-ddTHH:mm:ss.fff}" // o: Round-trip date/time pattern.
                            : property.ExcelFormatAttribute.DataFormatString;

                        var valueDate = String.Format(
                            CultureInfo.InvariantCulture
                            , format
                            , value
                            );

                        result.AppendFormat(
                            "\r\n    <Cell ss:StyleID=\"shortdate\"><Data ss:Type=\"DateTime\">{0}</Data></Cell>"
                            , valueDate
                        );
                    }
                    else
                    {
                        result.AppendFormat(
                            "\r\n    <Cell{1}><Data ss:Type=\"String\">{0}</Data></Cell>"
                            , value
                            , cellAttribute
                        );
                    }
                }
                catch (Exception ex)
                {
                    result.AppendFormat(
                        "\r\n    <Cell><Data ss:Type=\"String\">{0}</Data></Cell>"
                        , "** EXPORT ERROR **"
                    );
                    //ex.LogException(Logger);
                    Console.WriteLine("{0}", ex);
                }
            }

            result.AppendFormat("\r\n   </Row>");

            return result.ToString();
        }


        public string ConvertEnd()
        {
            return "\r\n  </Table>\r\n </Worksheet>\r\n</Workbook>";
        }


        public ExcelConverterV2(string currentWebsiteName)
        {
            if (String.IsNullOrWhiteSpace(currentWebsiteName)) { throw new ArgumentNullException(nameof(currentWebsiteName)); }
            this.CurrentWebsiteName = currentWebsiteName;
        }

    }
}
