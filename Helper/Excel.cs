using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ironclad.Helper
{
    class Excel
    {
        public static DataTable GetDataTableFromExcel(string path, string worksheetName, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                try
                {
                    using (var stream = File.OpenRead(path))
                    {
                        pck.Load(stream);
                    }
                } catch
                {
                    MessageBox.Show("config.xlsx is already opened by another program. Please close it before running Ironclad!", "Error");
                    Environment.Exit(1);
                }
                var ws = pck.Workbook.Worksheets[worksheetName];
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column { 0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }
    }
}
