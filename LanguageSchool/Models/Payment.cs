using System;

namespace LanguageSchool.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int ClientInGroupId { get; set; }
    
    public int GroupId { get; set; }
    
    public string GroupName { get; set; }
    
    public int ClientId { get; set; }
    
    public string ClientName { get; set; }

    public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;

    public double Count { get; set; }
}
