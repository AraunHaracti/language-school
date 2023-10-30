using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class ClientLanguage
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int ProficiencyLevelId { get; set; }

    public string? LastExperience { get; set; }

    public string? Needs { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ProficiencyLevel ProficiencyLevel { get; set; } = null!;
}
