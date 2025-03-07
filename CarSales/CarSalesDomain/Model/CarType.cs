using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class CarType : Entity
{
    [Required(ErrorMessage = "Тип автомобіля є обов'язковим.")]
    [StringLength(50, ErrorMessage = "Тип автомобіля не може перевищувати 50 символів.")]
    [Display(Name = "Тип автомобіля")]
    public string CarType1 { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
