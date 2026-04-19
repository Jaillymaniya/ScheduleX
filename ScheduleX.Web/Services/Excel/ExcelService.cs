using ClosedXML.Excel;
using ScheduleX.Web.DTOs;
using ScheduleX.Web.Models.Template;

namespace ScheduleX.Web.Services.Excel
{
    public class ExcelService : IExcelService
    {
        public byte[] GenerateExcel(List<PreviewDto> data)
        {
            using var wb = new XLWorkbook();

            // ✅ GROUP BY DIVISION
            var grouped = data.GroupBy(x => x.Division);

            foreach (var divisionGroup in grouped)
            {
                var ws = wb.Worksheets.Add(divisionGroup.Key); // Sheet name

                var style = new TemplateStyle
                {
                    headerBg = "#1e293b",
                    headerText = "#ffffff",
                    bodyBg = "#ffffff",
                    bodyText = "#111827",
                    borderColor = "#cbd5e1",
                    cellPadding = "8px",
                    fontSize = "14px",
                    showRoom = true,
                    showFaculty = true
                };

                var divData = divisionGroup.ToList();

                var days = divData.Select(x => x.Day).Distinct().OrderBy(x => x).ToList();
                var slots = divData.Select(x => x.Slot).Distinct().OrderBy(x => x).ToList();

                // ================= HEADER =================
                ws.Cell(1, 1).Value = "Time";

                for (int i = 0; i < days.Count; i++)
                {
                    ws.Cell(1, i + 2).Value = GetDayName(days[i]);
                }

                var headerRange = ws.Range(1, 1, 1, days.Count + 1);

                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml(style.headerBg);
                headerRange.Style.Font.FontColor = XLColor.FromHtml(style.headerText);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // ================= BODY =================
                int row = 2;

                foreach (var slot in slots)
                {
                    ws.Cell(row, 1).Value = $"Slot {slot}";
                    ws.Cell(row, 1).Style.Font.Bold = true;

                    for (int i = 0; i < days.Count; i++)
                    {
                        var cellData = divData
                            .FirstOrDefault(x => x.Day == days[i] && x.Slot == slot);

                        var cell = ws.Cell(row, i + 2);

                        if (cellData != null && cellData.Subject != "Free")
                        {
                            var text = cellData.Subject;

                            if (style.showFaculty && !string.IsNullOrEmpty(cellData.Faculty))
                                text += $"\n{cellData.Faculty}";

                            if (style.showRoom && !string.IsNullOrEmpty(cellData.Room))
                                text += $"\n{cellData.Room}";

                            cell.Value = text;
                        }
                        else
                        {
                            cell.Value = "Free";
                        }

                        // STYLE
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(style.bodyBg);
                        cell.Style.Font.FontColor = XLColor.FromHtml(style.bodyText);
                        cell.Style.Alignment.WrapText = true;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    row++;
                }

                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private string GetDayName(int day)
        {
            return day switch
            {
                1 => "Monday",
                2 => "Tuesday",
                3 => "Wednesday",
                4 => "Thursday",
                5 => "Friday",
                6 => "Saturday",
                _ => ""
            };
        }
    }
}