using System;
using System.Collections.Generic;

namespace ShradhaBookStore.Models;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Acronyms { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
