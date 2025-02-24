using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class Image : Entity
{
    [Display(Name = "Оголошення")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public int AdId { get; set; }

    [Display(Name = "Шлях до зображення")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string Path { get; set; } = null!;

    [Display(Name = "Оголошення")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public virtual Ad? Ad { get; set; } = null!;
}
