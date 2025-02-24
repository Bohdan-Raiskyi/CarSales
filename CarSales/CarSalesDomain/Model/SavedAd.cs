using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class SavedAd : Entity
{
    [Display(Name = "Користувач")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public int UserId { get; set; }

    [Display(Name = "Оголошення")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public int AdId { get; set; }

    [Display(Name = "Дата збереження")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public DateTime SavedDate { get; set; }

    [Display(Name = "Оголошення")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public virtual Ad? Ad { get; set; } = null!;

    [Display(Name = "Користувач")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public virtual User? User { get; set; } = null!;
}
