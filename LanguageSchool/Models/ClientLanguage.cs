namespace LanguageSchool.Models;

public partial class ClientLanguage
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int LanguageLevelId { get; set; }
    
    public int LanguageId { get; set; }
    
    public string LanguageLevelName { get; set; }
    
    public string LanguageName { get; set; }

    public string? LastExperience { get; set; }

    public string? Needs { get; set; }
}
