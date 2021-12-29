using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TimeClock.business.port.exporter;
using TimeClock.business.useCase.getSessionsTimeForADay;
using TimeClock.infrastructure.util;

namespace TimeClock.infrastructure.exporter.excelExporter
{
    public class ExcelExporter : IExporter
    {

        public GetSessionsTimeForADay GetSessionsTimeForADay { get; private set; }

        public ExcelExporter(GetSessionsTimeForADay getSessionsTimeForADay)
        {
            GetSessionsTimeForADay = getSessionsTimeForADay;
        }

        public void ExportFromAReferenceDate(DateTime date)
        {
            List<UserDetails> persons = new List<UserDetails>()
            {
                new UserDetails() {ID="1001", Name="ABCD", City ="City1", Country="USA"},
                new UserDetails() {ID="1002", Name="PQRS", City ="City2", Country="INDIA"},
                new UserDetails() {ID="1003", Name="XYZZ", City ="City3", Country="CHINA"},
                new UserDetails() {ID="1004", Name="LMNO", City ="City4", Country="UK"},
           };

            // Lets converts our object data to Datatable for a simplified logic.
            // Datatable is most easy way to deal with complex datatypes for easy reading and formatting.
            DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(persons), typeof(DataTable));
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TimeClock";
            string filePath = folderPath + "\\Result.xlsx";
            _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();

                ISheet excelSheet = workbook.CreateSheet("Sheet1");
                excelSheet.ProtectSheet("password");

                List<string> columns = new List<string>();
                IRow row = excelSheet.CreateRow(0);
                int columnIndex = 0;

                foreach (DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                    row.CreateCell(columnIndex).SetCellValue(column.ColumnName);
                    columnIndex++;
                }

                int rowIndex = 1;
                foreach (DataRow dsrow in table.Rows)
                {
                    row = excelSheet.CreateRow(rowIndex);
                    int cellIndex = 0;
                    foreach (string col in columns)
                    {
                        row.CreateCell(cellIndex).SetCellValue(dsrow[col].ToString());
                        cellIndex++;
                    }

                    rowIndex++;
                }

                DateTime currentDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime lastDayOfMonth = currentDayOfMonth.AddDays(DateTime.DaysInMonth(currentDayOfMonth.Year, currentDayOfMonth.Month) - 1);

                while (currentDayOfMonth <= lastDayOfMonth)
                {
                    var totalTimeForThisDay = FormatUtils.BuildTimerString(GetSessionsTimeForADay.Handle(new GetSessionsTimeForADayCommand(currentDayOfMonth)));

                    row = excelSheet.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(currentDayOfMonth.ToString("dd/MM/yyyy"));
                    row.CreateCell(1).SetCellValue(totalTimeForThisDay);
                    currentDayOfMonth = currentDayOfMonth.AddDays(1);
                    rowIndex++;
                }


                workbook.Write(fs);
            }

            SystemUtils.OpenExplorerOnFolder(folderPath);
        }
    }
}
