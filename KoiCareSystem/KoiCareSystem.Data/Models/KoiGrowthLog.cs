﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace KoiCareSystem.Data.Models;

public partial class KoiGrowthLog
{
    public int LogId { get; set; }

    public int? FishId { get; set; }

    public DateTime? GrowthDate { get; set; }

    public decimal? Size { get; set; }

    public decimal? Weight { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal RecommendedFoodAmount { get; set; }

    public virtual KoiFish Fish { get; set; }
}