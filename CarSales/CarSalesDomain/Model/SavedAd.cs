using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class SavedAd : Entity
{
    [Display(Name = "Користувач")]
    public int UserId { get; set; }

    [Display(Name = "Оголошення")]
    public int AdId { get; set; }

    [Display(Name = "Дата збереження")]
    public DateTime SavedDate { get; set; }

    [Display(Name = "Оголошення")]
    public virtual Ad? Ad { get; set; } = null!;

    [Display(Name = "Користувач")]
    public virtual User? User { get; set; } = null!;
}
