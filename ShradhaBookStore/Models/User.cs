using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShradhaBookStore.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Usertype { get; set; }

    public string? Password { get; set; }

    public string? Image { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }


    public string? Address { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
