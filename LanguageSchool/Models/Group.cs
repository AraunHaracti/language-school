using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Group
{
    public int Id { get; set; }

    public int Teacher { get; set; }

    public int CourseId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ClientInGroup> ClientInGroups { get; set; } = new List<ClientInGroup>();

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual Teacher TeacherNavigation { get; set; } = null!;
}
