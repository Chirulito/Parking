using System;
using System.Collections.Generic;

namespace Share.Models;

public partial class Log
{
    public int IdLog { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string Description { get; set; }

    public DateTime? Timestamp { get; set; }
}
