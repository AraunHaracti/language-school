﻿using System;
using System.Collections.Generic;

namespace LanguageSchool.Models;
public partial class TeacherLanguage
{
    public int Id { get; set; }

    public int Teacher { get; set; }

    public int ProficiencyLevelId { get; set; }

    public virtual ProficiencyLevel ProficiencyLevel { get; set; } = null!;

    public virtual Teacher TeacherNavigation { get; set; } = null!;
}