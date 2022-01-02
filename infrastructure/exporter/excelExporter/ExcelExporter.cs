using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using TimeClock.business.model.workSession;
using TimeClock.business.port.exporter;
using TimeClock.business.port.repository;
using TimeClock.business.useCase.getSessionsTimeForADay;
using TimeClock.infrastructure.util;

namespace TimeClock.infrastructure.exporter.excelExporter
{
    public class ExcelExporter : IExporter
    {

        public GetSessionsTimeForADay GetSessionsTimeForADay { get; private set; }
        private readonly IWorkSessionRepository _workSessionRepository;

        public ExcelExporter(GetSessionsTimeForADay getSessionsTimeForADay, IWorkSessionRepository workSessionRepository)
        {
            GetSessionsTimeForADay = getSessionsTimeForADay;
            _workSessionRepository = workSessionRepository;
        }

        public void ExportFromAReferenceDate(DateTime date)
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TimeClock";
            string filePath = folderPath + $"\\TimeClock_Export_{DateTime.Now:dd-MM-yyyy-HHmmss}.xlsx";
            _ = Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                List<WorkSession> monthSessions = _workSessionRepository.FindDistinctOneByMonth(5);

                foreach (WorkSession monthSession in monthSessions)
                {
                    ExportMonthSheet(workbook, monthSession.Date);
                }


                workbook.Write(fs);
            }

            SystemUtils.OpenExplorerOnFolder(folderPath);
        }

        private void ExportMonthSheet(IWorkbook workbook, DateTime refDate)
        {
            ISheet excelSheet = workbook.CreateSheet(refDate.ToString("MMMM yyyy"));
            excelSheet.ProtectSheet("password");

            TimeSpan weekTimeSpan = new TimeSpan();
            TimeSpan totalTimeSpan = new TimeSpan();
            DateTime currentDayOfMonth = new DateTime(refDate.Year, refDate.Month, 1);
            DateTime lastDayOfMonth = currentDayOfMonth.AddDays(DateTime.DaysInMonth(currentDayOfMonth.Year, currentDayOfMonth.Month) - 1);
            int rowIndex = 0;
            IRow row = excelSheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("Date");
            row.CreateCell(1).SetCellValue("Temps du jour");
            row.CreateCell(2).SetCellValue("Temps en fin de semaine / mois");
            rowIndex++;
            while (currentDayOfMonth <= lastDayOfMonth)
            {
                TimeSpan totalTimeForThisDay = GetSessionsTimeForADay.Handle(new GetSessionsTimeForADayCommand(currentDayOfMonth));
                weekTimeSpan = weekTimeSpan.Add(totalTimeForThisDay);
                totalTimeSpan = totalTimeSpan.Add(totalTimeForThisDay);
                string formattedTimeForThisDay = FormatUtils.BuildTimerString(totalTimeForThisDay);
                row = excelSheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(currentDayOfMonth.ToString("dd/MM/yyyy"));
                row.CreateCell(1).SetCellValue(formattedTimeForThisDay);

                if (!DateUtils.IsInTheSameWeek(currentDayOfMonth, currentDayOfMonth.AddDays(1)) || currentDayOfMonth == lastDayOfMonth)
                {
                    row.CreateCell(2).SetCellValue(FormatUtils.BuildTimerString(weekTimeSpan));
                    weekTimeSpan = new TimeSpan();
                }

                currentDayOfMonth = currentDayOfMonth.AddDays(1);
                rowIndex++;
            }

            row = excelSheet.CreateRow(++rowIndex);
            row.CreateCell(0).SetCellValue("Total");
            row.CreateCell(1).SetCellValue(FormatUtils.BuildTimerString(totalTimeSpan));

            excelSheet.AutoSizeColumn(0);
            excelSheet.AutoSizeColumn(1);
            excelSheet.AutoSizeColumn(2);
        }
    }
}
