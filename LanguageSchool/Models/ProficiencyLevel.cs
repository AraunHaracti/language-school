using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class ProficiencyLevel
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ClientLanguage> ClientLanguages { get; set; } = new List<ClientLanguage>();

    public virtual ICollection<Curse> Curses { get; set; } = new List<Curse>();

    public virtual Language Language { get; set; } = null!;

    public virtual ICollection<TeacherLanguage> TeacherLanguages { get; set; } = new List<TeacherLanguage>();
}
