using ScheduleX.Web.DTOs;
using ScheduleX.Web.Models.Template;

namespace ScheduleX.Web.Services.Excel
{
    public interface IExcelService
    {
        //byte[] GenerateExcel(List<PreviewDto> data);

        byte[] GenerateExcel(List<PreviewDto> data, TemplateStyle style);
    }
}
