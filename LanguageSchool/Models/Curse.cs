using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Curse
{
    public int Id { get; set; }

    public int ProficiencyLevelId { get; set; }

    public string? Name { get; set; }

    public string Info { get; set; } = null!;

    public double Price { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ProficiencyLevel ProficiencyLevel { get; set; } = null!;
}
