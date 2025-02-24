﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class User : Entity
{
    [Display(Name = "Ім'я користувача")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string UserName { get; set; } = null!;

    [Display(Name = "Пошта")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string Email { get; set; } = null!;

    [Display(Name = "Пароль")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string Password { get; set; } = null!;

    [Display(Name = "Телефон")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Дата створення")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();

    public virtual ICollection<SavedAd> SavedAds { get; set; } = new List<SavedAd>();
}
