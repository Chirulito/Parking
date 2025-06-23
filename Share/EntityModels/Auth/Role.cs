namespace Share.EntityModels.Auth;

public partial class Role
{
    public int IdRole { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<RoleResource> RolesResources { get; set; } = new List<RoleResource>();

    public virtual ICollection<UserRole> UsersRoles { get; set; } = new List<UserRole>();
}
