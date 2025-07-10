using System;
using System.Collections.Generic;
using Share.EntityModels.Auth;

namespace Share.EntityModels.Parking;

public partial class Vehicle
{
    public int IdVehicle { get; set; }

    public int IdUser { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public string? Color { get; set; }

    public bool? Accommodation { get; set; }

    public string? Type { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Occupancy> Occupancies { get; set; } = new List<Occupancy>();
}
