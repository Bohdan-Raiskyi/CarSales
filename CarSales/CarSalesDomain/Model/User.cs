using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CarSalesDomain.Model
{
    public partial class User : Entity
    {
        [Required(ErrorMessage = "Ім'я користувача є обов'язковим.")]
        [StringLength(50, ErrorMessage = "Ім'я користувача не може перевищувати 50 символів.")]
        [Display(Name = "Ім'я користувача")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Пошта є обов'язковою.")]
        [EmailAddress(ErrorMessage = "Некоректний формат електронної пошти.")]
        [StringLength(100, ErrorMessage = "Пошта не може перевищувати 100 символів.")]
        [Display(Name = "Пошта")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Пароль є обов'язковим.")]
        [Display(Name = "Пароль")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Пароль повинен містити від 8 до 20 символів.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
        ErrorMessage = "Пароль має містити хоча б одну велику літеру, одну малу, одну цифру та один спеціальний символ.")]
        public string Password { get; set; } = null!;

        [Phone(ErrorMessage = "Некоректний формат номера телефону.")]
        [StringLength(20, ErrorMessage = "Номер телефону не може перевищувати 20 символів.")]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Дата створення")]
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();

        public virtual ICollection<SavedAd> SavedAds { get; set; } = new List<SavedAd>();
    }
}

//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace CarSalesDomain.Model;

//public partial class User : Entity
//{
//    [Display(Name = "Ім'я користувача")]
//    public string UserName { get; set; } = null!;

//    [Display(Name = "Пошта")]
//    public string Email { get; set; } = null!;

//    [Display(Name = "Пароль")]
//    public string Password { get; set; } = null!;

//    [Display(Name = "Телефон")]
//    public string? PhoneNumber { get; set; }

//    [Display(Name = "Дата створення")]
//    public DateTime CreatedDate { get; set; }

//    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();

//    public virtual ICollection<SavedAd> SavedAds { get; set; } = new List<SavedAd>();
//}
