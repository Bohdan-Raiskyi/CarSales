using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class CarType : Entity
{
    [Display(Name = "Тип автомобіля")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string CarType1 { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
