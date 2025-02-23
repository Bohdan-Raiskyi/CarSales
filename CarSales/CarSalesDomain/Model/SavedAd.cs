using System;
using System.Collections.Generic;

namespace CarSalesDomain.Model;

public partial class SavedAd : Entity
{
    public int UserId { get; set; }

    public int AdId { get; set; }

    public DateTime SavedDate { get; set; }

    public virtual Ad? Ad { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
