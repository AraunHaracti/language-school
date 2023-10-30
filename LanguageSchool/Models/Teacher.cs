using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Teacher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateOnly Birthday { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<TeacherLanguage> TeacherLanguages { get; set; } = new List<TeacherLanguage>();
}
