using System;
using System.Collections.Generic;

namespace Share.Models;

public partial class Occupancy
{
    public int IdOccupancy { get; set; }

    public int IdSpot { get; set; }

    public string LicensePlate { get; set; } = null!;

    public bool? Accommodation { get; set; }

    public string? Type { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Spot IdSpotNavigation { get; set; } = null!;

}
