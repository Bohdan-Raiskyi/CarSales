using System;
using System.Collections.Generic;

namespace CarSalesDomain.Model;

public partial class Image : Entity
{
    public int AdId { get; set; }

    public string Path { get; set; } = null!;

    public virtual Ad? Ad { get; set; } = null!;
}
