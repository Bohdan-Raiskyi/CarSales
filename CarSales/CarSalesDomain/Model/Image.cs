using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class Image : Entity
{
    [Display(Name = "Оголошення")]
    public int AdId { get; set; }

    [Required(ErrorMessage = "Шлях до зображення є обов'язковим.")]
    [StringLength(50, ErrorMessage = "Шлях до зображення не може перевищувати 50 символів.")]
    [Display(Name = "Шлях до зображення")]
    public string Path { get; set; } = null!;

    [Display(Name = "Оголошення")]
    public virtual Ad? Ad { get; set; } = null!;
}
