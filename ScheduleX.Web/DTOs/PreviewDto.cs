namespace ScheduleX.Web.DTOs
{
    public class PreviewDto
    {
        public int Day { get; set; }
        public int Slot { get; set; }
        public string Subject { get; set; }
        public string Faculty { get; set; }
        public string Room { get; set; }
        public string Division { get; set; }

        public int EntryId { get; set; }
        public int? SubjectSemesterId { get; set; }
        public int? RoomId { get; set; }
    }
}
