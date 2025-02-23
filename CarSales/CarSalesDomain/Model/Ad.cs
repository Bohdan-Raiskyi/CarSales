using System;
using System.Collections.Generic;

namespace CarSalesDomain.Model;

public partial class Ad : Entity
{
    public int UserId { get; set; }

    public int TypeId { get; set; }

    public int BrandId { get; set; }

    public int PriceRangeId { get; set; }

    public int RegionId { get; set; }

    public decimal Price { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? SoldDate { get; set; }

    public virtual CarBrand? Brand { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual PriceRange? PriceRange { get; set; } = null!;

    public virtual Region? Region { get; set; } = null!;

    public virtual ICollection<SavedAd> SavedAds { get; set; } = new List<SavedAd>();

    public virtual CarType? Type { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
