namespace ScheduleX.Web.DTOs
{
    public class UpdateEntryDto
    {
        public int EntryId { get; set; }
        public int? SubjectSemesterId { get; set; }
        public int? RoomId { get; set; }
        public int UserId { get; set; }
    }
}