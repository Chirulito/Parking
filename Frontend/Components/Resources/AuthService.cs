public class AuthService
{
    public List<string> Roles { get; private set; } = new();

    public bool IsLoggedIn => Roles.Count > 0;

    public void SetRoles(List<string> roles)
    {
        Roles = roles;
        Console.WriteLine("Roles guardados en AuthService:");
        foreach (var role in Roles)
        {
            Console.WriteLine($"- {role}");
        }
    }

    public void Logout()
    {
        Roles.Clear();
    }

    public bool HasRole(string role) => Roles.Contains(role);
}
