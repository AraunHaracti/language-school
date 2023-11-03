using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Course
{
    public int Id { get; set; }

    public int ProficiencyLevelId { get; set; }

    public int LanguageId { get; set; }
    
    public string ProficiencyLevelName { get; set; }
    
    public string LanguageName { get; set; }
    
    public string? Name { get; set; }

    public string Info { get; set; } = null!;

    public double Price { get; set; }
}
