using System;
using System.Collections.Generic;

namespace Share.Models;

public partial class UsersRole
{
    public int IdUserRole { get; set; }

    public int IdUser { get; set; }

    public int IdRole { get; set; }

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
