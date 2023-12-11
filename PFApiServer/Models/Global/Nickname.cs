using System;
using System.Collections.Generic;

namespace PFApiServer.Models.Global;

public partial class Nickname
{
    public ulong SerialNo { get; set; }

    public ulong Uid { get; set; }

    public string Name { get; set; } = null!;

    public ulong AccountSerialNo { get; set; }
}
