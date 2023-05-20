using System.Data;
using System.IO;
using System;
using ClosedXML.Excel;

namespace api_pns.Models.Excel
{
    public class ReporteExcel
    {
        #region Generar informe plano sin diseño
        /// <summary>
        /// Generar reporte en Excel plano sin diseño
        /// </summary>
        /// <remarks>
        /// Método para obtener reporte en Excel basico a partir de información relacionada en una tabla. 
        /// </remarks>
        public string CreateSheet(DataTable tableData)
        {
            using var wb = new XLWorkbook();
            var sheet = wb.AddWorksheet("Report");

            var titleStyle = wb.Style;
            titleStyle.Font.Bold = true;
            titleStyle.Font.FontSize = 11;
            titleStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sheet.Row(1).Style = titleStyle;

            // Cargue de información
            var table = sheet.Cell("A1").InsertTable(tableData.AsEnumerable());
            table.Theme = XLTableTheme.None;
            table.ShowAutoFilter = false;
            sheet.Columns().AdjustToContents();

            //Creación de archivo excel
            MemoryStream excelStream = new MemoryStream();
            wb.SaveAs(excelStream);

            return Convert.ToBase64String(excelStream.ToArray());
        }
        #endregion
    }
}
