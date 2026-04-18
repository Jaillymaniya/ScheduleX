using ScheduleX.Web.DTOs;

namespace ScheduleX.Web.Services.Excel
{
    public interface IExcelService
    {
        byte[] GenerateExcel(List<PreviewDto> data);
    }
}
