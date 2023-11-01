using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class Payment
{
    public int Id { get; set; }

    public int ClientInGroup { get; set; }

    public DateTime Date { get; set; }

    public double Count { get; set; }
}
