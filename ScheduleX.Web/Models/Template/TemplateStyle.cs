namespace ScheduleX.Web.Models.Template
{
    public class TemplateStyle
    {
        public string headerBg { get; set; }
        public string headerText { get; set; }
        public string bodyBg { get; set; }
        public string bodyText { get; set; }
        public string borderColor { get; set; }
        public string cellPadding { get; set; }
        public string fontSize { get; set; }

        public bool showRoom { get; set; }
        public bool showFaculty { get; set; }
        public bool showSubjectCode { get; set; }
        public string titleAlign { get; set; }
    }
}