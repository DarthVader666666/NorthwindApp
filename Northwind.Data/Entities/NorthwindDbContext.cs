using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Data.Entities;

public partial class NorthwindDbContext : DbContext
{

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
