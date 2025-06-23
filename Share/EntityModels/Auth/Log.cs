namespace Share.EntityModels.Auth;

public partial class Log
{
    public int IdLog { get; set; }

    public int? IdUser { get; set; }

    public string? Action { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
