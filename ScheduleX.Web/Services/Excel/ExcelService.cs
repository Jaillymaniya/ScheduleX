namespace ScheduleX.Web.Services.Excel
{
    using ClosedXML.Excel;
    using ScheduleX.Web.DTOs;

    public class ExcelService : IExcelService
    {
        public byte[] GenerateExcel(List<PreviewDto> data)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Timetable");

            ws.Cell(1, 1).Value = "Day";
            ws.Cell(1, 2).Value = "Slot";
            ws.Cell(1, 3).Value = "Subject";
            ws.Cell(1, 4).Value = "Faculty";
            ws.Cell(1, 5).Value = "Room";
            ws.Cell(1, 6).Value = "Division";

            int row = 2;

            foreach (var d in data)
            {
                ws.Cell(row, 1).Value = d.Day;
                ws.Cell(row, 2).Value = d.Slot;
                ws.Cell(row, 3).Value = d.Subject;
                ws.Cell(row, 4).Value = d.Faculty;
                ws.Cell(row, 5).Value = d.Room;
                ws.Cell(row, 6).Value = d.Division;
                row++;
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
