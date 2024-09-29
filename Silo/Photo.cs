using System;
using System.Collections.Generic;

namespace Orleans.ShoppingCart.Silo;

public partial class Photo
{
    public int PhotoId { get; set; }

    public int ClientId { get; set; }

    public string FileName { get; set; } = null!;

    public string? FileType { get; set; }

    public string? Url { get; set; }

    public virtual Client Client { get; set; } = null!;
}
