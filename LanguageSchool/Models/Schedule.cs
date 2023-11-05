using System;

namespace LanguageSchool.Models;

public partial class Schedule
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    
    public string GroupName { get; set; }

    public string? About { get; set; }

    public DateTimeOffset Datetime { get; set; } = DateTimeOffset.Now;
}
