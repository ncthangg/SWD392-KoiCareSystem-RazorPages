﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiCareSystem.Data.Models;

public partial class Payment
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public decimal Total { get; set; }

    public long UserId { get; set; }

    public virtual Order Order { get; set; }

    public virtual User User { get; set; }
}