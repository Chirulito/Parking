using System;
using System.Collections.Generic;

namespace Share.Models;

public partial class Building
{
    public int IdBuilding { get; set; }

    public string Name { get; set; } = null!;

    public int? MaximumCapacity { get; set; }

    public virtual ICollection<Spot> Spots { get; set; } = new List<Spot>();
}
