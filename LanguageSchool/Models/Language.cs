using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Language
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProficiencyLevel> ProficiencyLevels { get; set; } = new List<ProficiencyLevel>();
}
