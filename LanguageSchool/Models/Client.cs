using System;

namespace LanguageSchool.Models;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateTimeOffset Birthday { get; set; } = DateTimeOffset.Now;

    public string? Phone { get; set; }

    public string? Email { get; set; }
}
