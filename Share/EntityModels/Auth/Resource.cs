namespace Share.EntityModels.Auth;

public partial class Resource
{
    public int IdResource { get; set; }

    public string Name { get; set; } = null!;

    public string? ResourceType { get; set; }

    public virtual ICollection<RoleResource> RolesResources { get; set; } = new List<RoleResource>();
}
