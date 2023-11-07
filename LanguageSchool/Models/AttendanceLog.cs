using System;

namespace LanguageSchool.Models;

public partial class AttendanceLog
{
    public int Id { get; set; }

    public int? ValueId { get; set; }

    public int ScheduleId { get; set; }

    public int ClientInGroupId { get; set; }
    
    public string GroupName { get; set; }
    
    public string ClientName { get; set; }
    
    public DateTimeOffset ScheduleDatetime { get; set; } = DateTimeOffset.Now;
    
    public string? Value { get; set; }
}
