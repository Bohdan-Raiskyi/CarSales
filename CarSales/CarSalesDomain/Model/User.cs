using System;
using System.Collections.Generic;

namespace CarSalesDomain.Model;

public partial class User : Entity
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();

    public virtual ICollection<SavedAd> SavedAds { get; set; } = new List<SavedAd>();
}
