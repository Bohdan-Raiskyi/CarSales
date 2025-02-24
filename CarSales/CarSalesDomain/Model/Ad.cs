using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class Ad : Entity
{
    [Display(Name = "Користувач")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public int UserId { get; set; }

    [Display(Name = "Тип авто")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public int TypeId { get; set; }

    [Display(Name = "Марка авто")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public int BrandId { get; set; }

    [Display(Name = "Ціновий діапазон")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public int PriceRangeId { get; set; }

    [Display(Name = "Регіон")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public int RegionId { get; set; }

    [Display(Name = "Ціна")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public decimal Price { get; set; }

    [Display(Name = "Назва оголошення")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public string Name { get; set; } = null!;

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    [Display(Name = "Дата створення")]
    public DateTime CreationDate { get; set; }

    [Display(Name = "Дата продажу")]
    public DateTime? SoldDate { get; set; }

    [Display(Name = "Марка")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public virtual CarBrand? Brand { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [Display(Name = "Ціновий діапазон")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public virtual PriceRange? PriceRange { get; set; } = null!;

    [Display(Name = "Регіон")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public virtual Region? Region { get; set; } = null!;

    public virtual ICollection<SavedAd> SavedAds { get; set; } = new List<SavedAd>();

    [Display(Name = "Тип авто")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public virtual CarType? Type { get; set; } = null!;

    [Display(Name = "Користувач")]
    [Required(ErrorMessage = "Поле не може бути порожнім")]
    public virtual User? User { get; set; } = null!;
}
