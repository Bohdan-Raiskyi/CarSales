using System;
using System.Collections.Generic;

namespace CarSalesDomain.Model;

public partial class Region : Entity
{
    public string RegName { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
