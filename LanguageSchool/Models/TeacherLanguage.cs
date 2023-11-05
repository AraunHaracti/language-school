namespace LanguageSchool.Models;

public partial class TeacherLanguage
{
    public int Id { get; set; }

    public int TeacherId { get; set; }

    public int LanguageLevelId { get; set; }
    
    public int LanguageId { get; set; }
    
    public string LanguageLevelName { get; set; }
    
    public string LanguageName { get; set; }
}
