using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class CarBrand : Entity
{
    [Display(Name = "Марка автомобіля")]
    public string BrandName { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
