using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateOnly Birthday { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<ClientInGroup> ClientInGroups { get; set; } = new List<ClientInGroup>();

    public virtual ICollection<ClientLanguage> ClientLanguages { get; set; } = new List<ClientLanguage>();
}
