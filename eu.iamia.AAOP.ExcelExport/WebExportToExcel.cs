using System;
using System.Collections.Generic;

namespace eu.iamia.AAOP.ExcelExport
{
    using System.IO;

    using eu.iamia.AAOP.CalculationEngine.Models;
    using eu.iamia.AAOP.ISP.Models;
    using eu.iamia.Util;
                             

    public class WebExportToExcel
    {
        private ExcelConverterV2<PaymentDetail> Converter
        { get; set; }

        public void Init()
        {
            Converter = new ExcelConverterV2<PaymentDetail>("AAOP");
        }


        public void Convert(StreamWriter outputStream, IEnumerable<IPaymentDetail> payload)
        {
             var header = Converter.ConvertHeader("Låneberegning", "Villy Ib Jørgensen", "", "Betalingsforløb");
            outputStream .WriteLine(header);

            var uri = new Uri("http://www.dmi.dk"); // only used if row contain hyperlnks

            foreach (var row in payload)
            {
                var exportRow = Converter.ConvertRow((PaymentDetail)row, uri);
                outputStream.WriteLine(exportRow);
            }

            var tail = Converter.ConvertEnd();
            outputStream.WriteLine(tail);
            outputStream.Flush();
        }

    }
}
