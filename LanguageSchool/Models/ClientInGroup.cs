using System;

namespace LanguageSchool.Models;

public partial class ClientInGroup
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public int ClientId { get; set; }
    
    public string ClientName { get; set; }
    
    public string ClientSurname { get; set; }
    
    public DateTimeOffset ClientBirthday { get; set; } = DateTimeOffset.Now; 
}
