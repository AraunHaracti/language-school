using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class TeacherLanguage
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int ProficiencyLevelId { get; set; }
    
    public int LanguageId { get; set; }
    
    public string ProficiencyLevelName { get; set; }
    
    public string LanguageName { get; set; }
}
