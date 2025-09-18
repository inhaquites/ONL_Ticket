using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using System.Data;

namespace Lenovo.NAT.Services
{
    public interface IExcelExportService
    {
        IWorkbook WriteExcelWithNPOI<T>(List<T> data);

        IWorkbook WriteExcelWithNPOI<T>(List<T> data, List<string> columnNames);
    }
    public class ExcelExportService: IExcelExportService
    {
        public IWorkbook WriteExcelWithNPOI<T>(List<T> data)
        {
            var workbook = new XSSFWorkbook();

            var datatable = ConvertListToDataTable(data);

            var sheet1 = workbook.CreateSheet("Sheet 1");

            var row0 = sheet1.CreateRow(0);

            for (var j = 0; j < datatable.Columns.Count; j++)
            {
                var cell = row0.CreateCell(j);
                var columnName = datatable.Columns[j].ToString();

                cell.SetCellValue(columnName);
            }

            for (var rowIndex = 0; rowIndex < datatable.Rows.Count; rowIndex++)
            {
                var row = sheet1.CreateRow(rowIndex + 1);

                for (var columnIndex = 0; columnIndex < datatable.Columns.Count; columnIndex++)
                {
                    var cell = row.CreateCell(columnIndex);
                    var columnName = datatable.Columns[columnIndex].ToString();

                    cell.SetCellValue(datatable.Rows[rowIndex][columnName].ToString());
                    cell.CellStyle.WrapText = true; 
                }
            }
            
            for (var i = 0; i < 5; i++)
                for (var j = 0; j < row0.LastCellNum; j++)
                    sheet1.AutoSizeColumn(j);

            return workbook;
        }

        public IWorkbook WriteExcelWithNPOI<T>(List<T> data, List<string> columnNames)
        {
            var workbook = new XSSFWorkbook();

            var datatable = ConvertListToDataTable(data);

            var sheet1 = workbook.CreateSheet("Sheet 1");

            var row0 = sheet1.CreateRow(0);

            for (var j = 0; j < columnNames.Count; j++)
            {
                var cell = row0.CreateCell(j);
                var columnName = columnNames[j].ToString();

                cell.SetCellValue(columnName);
            }

            for (var rowIndex = 0; rowIndex < datatable.Rows.Count; rowIndex++)
            {
                var row = sheet1.CreateRow(rowIndex + 1);

                for (var columnIndex = 0; columnIndex < datatable.Columns.Count; columnIndex++)
                {
                    var cell = row.CreateCell(columnIndex);
                    var columnName = datatable.Columns[columnIndex].ToString();

                    cell.SetCellValue(datatable.Rows[rowIndex][columnName].ToString());
                    cell.CellStyle.WrapText = true;
                }
            }

            for (var i = 0; i < 5; i++)
                for (var j = 0; j < row0.LastCellNum; j++)
                    sheet1.AutoSizeColumn(j);

            return workbook;
        }

        private DataTable ConvertListToDataTable<T>(IList<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                var row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                
                table.Rows.Add(row);
            }
            
            return table;
        }
    }
}
