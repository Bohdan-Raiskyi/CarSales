using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class Region : Entity
{
    [Display(Name = "Регіон")]
    public string RegName { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
