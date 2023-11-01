using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Group
{
    public int Id { get; set; }

    public int Teacher { get; set; }

    public int CourseId { get; set; }

    public string Name { get; set; } = null!;
}
