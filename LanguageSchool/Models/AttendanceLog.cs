namespace LanguageSchool.Models;

public partial class AttendanceLog
{
    public int Id { get; set; }

    public int? ValueId { get; set; }

    public int ScheduleId { get; set; }

    public int ClientInGroupId { get; set; }
}
