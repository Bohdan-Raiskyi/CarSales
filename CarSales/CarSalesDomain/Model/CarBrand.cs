using System;
using System.Collections.Generic;

namespace CarSalesDomain.Model;

public partial class CarBrand : Entity
{
    public string BrandName { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
