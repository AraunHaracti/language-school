using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }
}
