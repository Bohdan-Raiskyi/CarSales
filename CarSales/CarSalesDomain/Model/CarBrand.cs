using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class CarBrand : Entity
{
    [Required(ErrorMessage = "Марка автомобіля є обов'язкова.")]
    [StringLength(50, ErrorMessage = "Марка автомобіля не може перевищувати 50 символів.")]
    [Display(Name = "Марка автомобіля")]
    public string BrandName { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
