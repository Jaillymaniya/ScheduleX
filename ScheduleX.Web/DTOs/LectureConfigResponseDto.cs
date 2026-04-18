namespace ScheduleX.Web.DTOs
{
    public class LectureConfigResponseDto
    {
        public int LecturesPerDay { get; set; }

        public List<LectureConfigRowDto> Rows { get; set; } = new();
    }

}
