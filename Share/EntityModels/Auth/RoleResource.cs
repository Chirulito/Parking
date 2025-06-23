namespace Share.EntityModels.Auth;

public partial class RoleResource
{
    public int IdRoleResource { get; set; }

    public int IdRole { get; set; }

    public int IdResource { get; set; }

    public virtual Resource IdResourceNavigation { get; set; } = null!;

    public virtual Role IdRoleNavigation { get; set; } = null!;
}
