﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Entities;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public int? AssignedBy { get; set; }

    public bool? IsPrimary { get; set; }

    public virtual User? AssignedByNavigation { get; set; }

    public virtual Role Role { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
