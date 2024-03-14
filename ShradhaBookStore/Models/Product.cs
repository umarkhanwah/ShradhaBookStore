using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShradhaBookStore.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Image { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }


    public int? CategoryId { get; set; }

    public int? ManufacturerId { get; set; }

    public int? Price { get; set; }

    public string Description { get; set; } = null!;

    public string Acronym { get; set; } = null!;

    public int Quantity { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category? Category { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
