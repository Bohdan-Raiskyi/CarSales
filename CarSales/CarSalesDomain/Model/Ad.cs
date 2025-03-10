using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarSalesDomain.Model;

public partial class Ad : Entity
{
    [Display(Name = "Користувач")]
    public int UserId { get; set; }

    [Display(Name = "Тип авто")]
    public int TypeId { get; set; }

    [Display(Name = "Марка авто")]
    public int BrandId { get; set; }

    [Display(Name = "Ціновий діапазон")]
    public int PriceRangeId { get; set; }

    [Display(Name = "Регіон")]
    public int RegionId { get; set; }

    [Display(Name = "Ціна")]
    [Required(ErrorMessage = "Поле 'Ціна' є обов'язковим.")]
    [Range(0, int.MaxValue, ErrorMessage = "Ціна - додатнє ціле число.")]
    public int Price { get; set; }


    [Display(Name = "Назва оголошення")]
    [Required(ErrorMessage = "Поле 'Назва оголошення' є обов'язковим.")]
    [StringLength(50, ErrorMessage = "Назва оголошення не може перевищувати 50 символів.")]
    public string Name { get; set; } = null!;

    [Display(Name = "Опис")]
    [StringLength(4000, ErrorMessage = "Опис не може перевищувати 4000 символів.")]
    public string? Description { get; set; }

    [Display(Name = "Дата створення")]
    public DateTime CreationDate { get; set; }

    [Display(Name = "Дата продажу")]
    public DateTime? SoldDate { get; set; }

    [Display(Name = "Марка")]
    public virtual CarBrand? Brand { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [Display(Name = "Ціновий діапазон")]
    public virtual PriceRange? PriceRange { get; set; } = null!;

    [Display(Name = "Регіон")]
    public virtual Region? Region { get; set; } = null!;

    public virtual ICollection<SavedAd> SavedAds { get; set; } = new List<SavedAd>();

    [Display(Name = "Тип авто")]
    public virtual CarType? Type { get; set; } = null!;

    [Display(Name = "Користувач")]
    public virtual User? User { get; set; } = null!;
}
