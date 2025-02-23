using System;
using System.Collections.Generic;

namespace CarSalesDomain.Model;

public partial class CarType : Entity
{
    public string CarType1 { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
