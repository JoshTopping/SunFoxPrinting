using System;
using System.Collections.Generic;

namespace Orleans.ShoppingCart.Silo;

public partial class Client
{
    public int ClientId { get; set; }

    public string ClientName { get; set; } = null!;

    public string? ClientPhone { get; set; }

    public string? ClientEmail { get; set; }

    public virtual Photo? Photo { get; set; }
}
