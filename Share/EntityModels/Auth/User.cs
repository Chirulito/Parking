namespace Share.EntityModels.Auth;

public partial class User
{
    public int IdUser { get; set; }

    public string Username { get; set; } = null!;

    public byte[] Password { get; set; } = null!;

    public byte[] Salt { get; set; } = null!;

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<UserRole> UsersRoles { get; set; } = new List<UserRole>();
}
