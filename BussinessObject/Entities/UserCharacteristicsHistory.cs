using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class UserCharacteristicsHistory
{
    public int CharacteristicHistoryId { get; set; }

    public int? CharacteristicId { get; set; }

    public string? Data { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual UserCharacteristic? Characteristic { get; set; }
}
