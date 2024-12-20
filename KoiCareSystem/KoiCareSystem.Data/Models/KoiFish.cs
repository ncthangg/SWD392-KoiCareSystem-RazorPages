﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiCareSystem.Data.Models;

public partial class KoiFish
{
    public int FishId { get; set; }

    public string FishName { get; set; }

    public int? UserId { get; set; }

    public int? PondId { get; set; }

    public string ImageUrl { get; set; }

    public string BodyShape { get; set; }

    public int? Age { get; set; }

    public decimal? Size { get; set; }

    public decimal? Weight { get; set; }

    public string Gender { get; set; }

    public string Origin { get; set; }

    public decimal? Price { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<KoiGrowthLog> KoiGrowthLogs { get; set; } = new List<KoiGrowthLog>();

    public virtual Pond Pond { get; set; }

    public virtual User User { get; set; }
}