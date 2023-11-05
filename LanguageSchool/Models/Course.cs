namespace LanguageSchool.Models;

public partial class Course
{
    public int Id { get; set; }

    public int LanguageLevelId { get; set; }

    public int LanguageId { get; set; }
    
    public string LanguageLevelName { get; set; }
    
    public string LanguageName { get; set; }
    
    public string Name { get; set; }

    public string? Info { get; set; }

    public double? Price { get; set; }
}
