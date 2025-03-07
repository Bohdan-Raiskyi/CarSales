using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class Region : Entity
{
    [Required(ErrorMessage = "Регіон є обов'язковим.")]
    [StringLength(50, ErrorMessage = "Регіон не може перевищувати 50 символів.")]
    [Display(Name = "Регіон")]
    public string RegName { get; set; } = null!;

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
}
