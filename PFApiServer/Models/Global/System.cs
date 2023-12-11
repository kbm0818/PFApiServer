using System;
using System.Collections.Generic;

namespace PFApiServer.Models.Global;

public partial class System
{
    public uint Status { get; set; }

    public bool Maintenance { get; set; }

    public string Version { get; set; } = null!;

    public float MajorVersion { get; set; }

    public sbyte MinorVersion { get; set; }
}
