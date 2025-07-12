public class AuthService
{
    public List<string> Roles { get; private set; } = new();

    public string Id { get; private set; }
    public string Name { get; private set; }


    public void SetUser(string id, string nombre, List<string> roles)
    {
        Id = id;
        Name = nombre;
        Roles = roles;

        Console.WriteLine($"\nUser: {Name} (id: {Id})");
        Console.WriteLine($"Roles in AuthService for {Name}:");

        foreach (var role in Roles)
        {
            Console.WriteLine($"- {role}");
        }
    }

    public void Logout()
    {
        Roles.Clear();
        Id = null;
        Name = null;
    }

    public bool HasRole(string role) => Roles.Contains(role);
}
