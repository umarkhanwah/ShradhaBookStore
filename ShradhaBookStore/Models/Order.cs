using System;
using System.Collections.Generic;

namespace ShradhaBookStore.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public int PaidAmount { get; set; }

    public string Status { get; set; } = null!;

    public int Payment { get; set; }

    public int Quantity { get; set; }

    public string Location { get; set; } = null!;

    public string ReceiverName { get; set; } = null!;

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
