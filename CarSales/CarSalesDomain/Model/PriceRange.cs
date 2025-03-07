using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class PriceRange : Entity
{
    [Required(ErrorMessage = "Ціновий діапазон є обов'язковим.")]
    [StringLength(50, ErrorMessage = "Ціновий діапазон не може перевищувати 50 символів.")]
    [Display(Name = "Ціновий діапазон")]
    public string RangeLabel { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
