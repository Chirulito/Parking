using System;
using System.Collections.Generic;

namespace Share.Models;

public partial class Role
{
    public int IdRole { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<UsersRole> UsersRoles { get; set; } = new List<UsersRole>();
}
