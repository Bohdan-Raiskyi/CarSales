using System;
using System.Collections.Generic;

namespace CarSalesDomain.Model;

public partial class PriceRange : Entity
{
    public string RangeLabel { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
