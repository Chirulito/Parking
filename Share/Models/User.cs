using System;
using System.Collections.Generic;

namespace Share.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Identification { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public byte[] Password { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public virtual ICollection<UsersRole> UsersRoles { get; set; } = new List<UsersRole>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
