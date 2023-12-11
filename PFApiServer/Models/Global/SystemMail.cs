using System;
using System.Collections.Generic;

namespace PFApiServer.Models.Global;

public partial class SystemMail
{
    public ulong SerialNo { get; set; }

    public uint MailType { get; set; }

    public string Title { get; set; } = null!;

    public string SubTitle { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public string? ItemList { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime ExpireDate { get; set; }

    public uint RemainDays { get; set; }
}
