using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class UserCharacteristic
{
    public int CharacteristicId { get; set; }

    public int? UserId { get; set; }

    public string? Source { get; set; }

    public string? Data { get; set; }

    public string? Metadata { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<UserCharacteristicsHistory> UserCharacteristicsHistories { get; set; } = new List<UserCharacteristicsHistory>();
}
