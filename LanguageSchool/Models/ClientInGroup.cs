using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class ClientInGroup
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int ClientId { get; set; }

    public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();

    public virtual Client Client { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
