using System;
using System.Collections.Generic;

namespace Share.EntityModels.Parking;

public partial class Spot
{
    public int IdSpot { get; set; }

    public int IdBuilding { get; set; }

    public string Code { get; set; } = null!;

    public string? Type { get; set; }

    public virtual Building IdBuildingNavigation { get; set; } = null!;

    public virtual ICollection<Occupancy> Occupancies { get; set; } = new List<Occupancy>();
}
