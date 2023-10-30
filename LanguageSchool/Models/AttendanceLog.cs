using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;

public partial class AttendanceLog
{
    public int Id { get; set; }

    public int? ValueId { get; set; }

    public int ScheduleId { get; set; }

    public int ClientInGroupId { get; set; }

    public virtual ClientInGroup ClientInGroup { get; set; } = null!;

    public virtual Schedule Schedule { get; set; } = null!;

    public virtual Value? Value { get; set; }
}
