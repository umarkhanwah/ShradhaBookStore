﻿using System;
using System.Collections.Generic;

namespace ShradhaBookStore.Models;

public partial class Review
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int? Stars { get; set; }

    public string? Comment { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
