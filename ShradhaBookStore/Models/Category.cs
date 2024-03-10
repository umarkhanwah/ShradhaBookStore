using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShradhaBookStore.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentCategoryId { get; set; }

    public string Description { get; set; } = null!;

    public string? Status { get; set; }

    public string? Image { get; set; }
    [NotMapped]
    public IFormFile ImageFile { get; set; }


    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
