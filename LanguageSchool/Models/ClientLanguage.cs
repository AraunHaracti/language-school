using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class ClientLanguage
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int ProficiencyLevelId { get; set; }
    
    public int LanguageId { get; set; }
    
    public string ProficiencyLevelName { get; set; }
    
    public string LanguageName { get; set; }

    public string? LastExperience { get; set; }

    public string? Needs { get; set; }
}
