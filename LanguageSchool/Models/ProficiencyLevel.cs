using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class ProficiencyLevel
{
    public int Id { get; set; }

    public int LanguageId { get; set; }

    public string Name { get; set; } = null!;
}
