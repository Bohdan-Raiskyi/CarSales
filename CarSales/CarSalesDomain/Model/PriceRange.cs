using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class PriceRange : Entity
{
    [Display(Name = "Ціновий діапазон")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string RangeLabel { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
