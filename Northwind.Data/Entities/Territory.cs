using System;
using System.Collections.Generic;

namespace Northwind.Data.Entities;

public partial class Territory
{
    public string TerritoryId { get; set; } = null!;

    public string TerritoryDescription { get; set; } = null!;

    public int RegionId { get; set; }

    public virtual Region Region { get; set; } = null!;

    public virtual ICollection<Seller> Sellers { get; set; } = new List<Seller>();
}
