﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiCareSystem.Data.Models;

public partial class OrderItem
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public long Quantity { get; set; }

    public long Price { get; set; }

    public virtual Order Order { get; set; }

    public virtual Product Product { get; set; }
}