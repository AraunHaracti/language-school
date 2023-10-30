﻿using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Value
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();
}
