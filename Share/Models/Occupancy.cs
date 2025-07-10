using System;
using System.Collections.Generic;

namespace Share.Models;

public partial class Occupancy
{
    public int IdOccupancy { get; set; }

    public int IdSpot { get; set; }

    public int IdVehicle { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Spot IdSpotNavigation { get; set; } = null!;

    public virtual Vehicle IdVehicleNavigation { get; set; } = null!;
}
