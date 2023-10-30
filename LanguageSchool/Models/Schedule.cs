using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Schedule
{
    public int Id { get; set; }

    public int? GroupId { get; set; }

    public string? About { get; set; }

    public DateTime Begin { get; set; }

    public DateTime End { get; set; }

    public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();

    public virtual Group? Group { get; set; }
}
