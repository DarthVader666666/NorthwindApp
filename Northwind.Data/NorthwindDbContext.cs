﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Northwind.Data.Entities;

namespace Northwind.Data
{
    public class NorthwindDbContext : IdentityDbContext<NorthwindUser>
    {
        public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AlphabeticalListOfProduct> AlphabeticalListOfProducts { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<CategorySalesFor1997> CategorySalesFor1997s { get; set; }

        public virtual DbSet<CurrentProductList> CurrentProductLists { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<CustomerAndSuppliersByCity> CustomerAndSuppliersByCities { get; set; }

        public virtual DbSet<CustomerDemographic> CustomerDemographics { get; set; }

        public virtual DbSet<Seller> Sellers { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        public virtual DbSet<OrderDetailsExtended> OrderDetailsExtendeds { get; set; }

        public virtual DbSet<OrderSubtotal> OrderSubtotals { get; set; }

        public virtual DbSet<OrdersQry> OrdersQries { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductSalesFor1997> ProductSalesFor1997s { get; set; }

        public virtual DbSet<ProductsAboveAveragePrice> ProductsAboveAveragePrices { get; set; }

        public virtual DbSet<ProductsByCategory> ProductsByCategories { get; set; }

        public virtual DbSet<QuarterlyOrder> QuarterlyOrders { get; set; }

        public virtual DbSet<Region> Regions { get; set; }

        public virtual DbSet<SalesByCategory> SalesByCategories { get; set; }

        public virtual DbSet<SalesTotalsByAmount> SalesTotalsByAmounts { get; set; }

        public virtual DbSet<Shipper> Shippers { get; set; }

        public virtual DbSet<SummaryOfSalesByQuarter> SummaryOfSalesByQuarters { get; set; }

        public virtual DbSet<SummaryOfSalesByYear> SummaryOfSalesByYears { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        public virtual DbSet<Territory> Territories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                optionsBuilder.UseInMemoryDatabase("Northwind_Azure");
            }
            else
            { 
                optionsBuilder.UseSqlServer();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AlphabeticalListOfProduct>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Alphabetical list of products");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
                entity.Property(e => e.CategoryName).HasMaxLength(15);
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.QuantityPerUnit).HasMaxLength(20);
                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
                entity.Property(e => e.UnitPrice).HasColumnType("money");
            });

            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.CategoryName, "CategoryName");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
                entity.Property(e => e.CategoryName).HasMaxLength(15);
                entity.Property(e => e.Description).HasColumnType("ntext");
                entity.Property(e => e.Picture).HasColumnType("image");
            });

            builder.Entity<CategorySalesFor1997>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Category Sales for 1997");

                entity.Property(e => e.CategoryName).HasMaxLength(15);
                entity.Property(e => e.CategorySales).HasColumnType("money");
            });

            builder.Entity<CurrentProductList>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Current Product List");

                entity.Property(e => e.ProductId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ProductID");
                entity.Property(e => e.ProductName).HasMaxLength(40);
            });

            builder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.City, "City");

                entity.HasIndex(e => e.CompanyName, "CompanyName");

                entity.HasIndex(e => e.PostalCode, "PostalCode");

                entity.HasIndex(e => e.Region, "Region");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(5)
                    .IsFixedLength()
                    .HasColumnName("CustomerID");
                entity.Property(e => e.Address).HasMaxLength(60);
                entity.Property(e => e.City).HasMaxLength(15);
                entity.Property(e => e.CompanyName).HasMaxLength(40);
                entity.Property(e => e.ContactName).HasMaxLength(30);
                entity.Property(e => e.ContactTitle).HasMaxLength(30);
                entity.Property(e => e.Country).HasMaxLength(15);
                entity.Property(e => e.Fax).HasMaxLength(24);
                entity.Property(e => e.Phone).HasMaxLength(24);
                entity.Property(e => e.PostalCode).HasMaxLength(10);
                entity.Property(e => e.Region).HasMaxLength(15);

                entity.HasMany(d => d.CustomerTypes).WithMany(p => p.Customers)
                    .UsingEntity<Dictionary<string, object>>(
                        "CustomerCustomerDemo",
                        r => r.HasOne<CustomerDemographic>().WithMany()
                            .HasForeignKey("CustomerTypeId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_CustomerCustomerDemo"),
                        l => l.HasOne<Customer>().WithMany()
                            .HasForeignKey("CustomerId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_CustomerCustomerDemo_Customers"),
                        j =>
                        {
                            j.HasKey("CustomerId", "CustomerTypeId").IsClustered(false);
                            j.ToTable("CustomerCustomerDemo");
                            j.IndexerProperty<string>("CustomerId")
                                .HasMaxLength(5)
                                .IsFixedLength()
                                .HasColumnName("CustomerID");
                            j.IndexerProperty<string>("CustomerTypeId")
                                .HasMaxLength(10)
                                .IsFixedLength()
                                .HasColumnName("CustomerTypeID");
                        });
            });

            builder.Entity<CustomerAndSuppliersByCity>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Customer and Suppliers by City");

                entity.Property(e => e.City).HasMaxLength(15);
                entity.Property(e => e.CompanyName).HasMaxLength(40);
                entity.Property(e => e.ContactName).HasMaxLength(30);
                entity.Property(e => e.Relationship)
                    .HasMaxLength(9)
                    .IsUnicode(false);
            });

            builder.Entity<CustomerDemographic>(entity =>
            {
                entity.HasKey(e => e.CustomerTypeId).IsClustered(false);

                entity.Property(e => e.CustomerTypeId)
                    .HasMaxLength(10)
                    .IsFixedLength()
                    .HasColumnName("CustomerTypeID");
                entity.Property(e => e.CustomerDesc).HasColumnType("ntext");
            });

            builder.Entity<Seller>(entity =>
            {
                entity.HasIndex(e => e.LastName, "LastName");

                entity.HasIndex(e => e.PostalCode, "PostalCode");

                entity.Property(e => e.SellerId).HasColumnName("SellerID");
                entity.Property(e => e.Address).HasMaxLength(60);
                entity.Property(e => e.BirthDate).HasColumnType("datetime");
                entity.Property(e => e.City).HasMaxLength(15);
                entity.Property(e => e.Country).HasMaxLength(15);
                entity.Property(e => e.Extension).HasMaxLength(4);
                entity.Property(e => e.FirstName).HasMaxLength(10);
                entity.Property(e => e.HireDate).HasColumnType("datetime");
                entity.Property(e => e.HomePhone).HasMaxLength(24);
                entity.Property(e => e.LastName).HasMaxLength(20);
                entity.Property(e => e.Notes).HasColumnType("ntext");
                entity.Property(e => e.Photo).HasColumnType("image");
                entity.Property(e => e.PhotoPath).HasMaxLength(255);
                entity.Property(e => e.PostalCode).HasMaxLength(10);
                entity.Property(e => e.Region).HasMaxLength(15);
                entity.Property(e => e.Title).HasMaxLength(30);
                entity.Property(e => e.TitleOfCourtesy).HasMaxLength(25);

                entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation)
                    .HasForeignKey(d => d.ReportsTo)
                    .HasConstraintName("FK_Sellers_Sellers");

                entity.HasMany(d => d.Territories).WithMany(p => p.Sellers)
                    .UsingEntity<Dictionary<string, object>>(
                        "EmployeeTerritory",
                        r => r.HasOne<Territory>().WithMany()
                            .HasForeignKey("TerritoryId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_SellerTerritories_Territories"),
                        l => l.HasOne<Seller>().WithMany()
                            .HasForeignKey("SellerId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_SellerTerritories_Sellers"),
                        j =>
                        {
                            j.HasKey("SellerId", "TerritoryId").IsClustered(false);
                            j.ToTable("SellerTerritories");
                            j.IndexerProperty<int>("SellerId").HasColumnName("SellerID");
                            j.IndexerProperty<string>("TerritoryId")
                                .HasMaxLength(20)
                                .HasColumnName("TerritoryID");
                        }).ToTable("Sellers");
            });

            builder.Entity<Invoice>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Invoices");

                entity.Property(e => e.Address).HasMaxLength(60);
                entity.Property(e => e.City).HasMaxLength(15);
                entity.Property(e => e.Country).HasMaxLength(15);
                entity.Property(e => e.CustomerId)
                    .HasMaxLength(5)
                    .IsFixedLength()
                    .HasColumnName("CustomerID");
                entity.Property(e => e.CustomerName).HasMaxLength(40);
                entity.Property(e => e.ExtendedPrice).HasColumnType("money");
                entity.Property(e => e.Freight).HasColumnType("money");
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.PostalCode).HasMaxLength(10);
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.Region).HasMaxLength(15);
                entity.Property(e => e.RequiredDate).HasColumnType("datetime");
                entity.Property(e => e.Salesperson).HasMaxLength(31);
                entity.Property(e => e.ShipAddress).HasMaxLength(60);
                entity.Property(e => e.ShipCity).HasMaxLength(15);
                entity.Property(e => e.ShipCountry).HasMaxLength(15);
                entity.Property(e => e.ShipName).HasMaxLength(40);
                entity.Property(e => e.ShipPostalCode).HasMaxLength(10);
                entity.Property(e => e.ShipRegion).HasMaxLength(15);
                entity.Property(e => e.ShippedDate).HasColumnType("datetime");
                entity.Property(e => e.ShipperName).HasMaxLength(40);
                entity.Property(e => e.UnitPrice).HasColumnType("money");
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.CustomerId, "CustomerID");

                entity.HasIndex(e => e.CustomerId, "CustomersOrders");

                entity.HasIndex(e => e.SellerId, "SellerID");

                entity.HasIndex(e => e.SellerId, "SellersOrders");

                entity.HasIndex(e => e.OrderDate, "OrderDate");

                entity.HasIndex(e => e.ShipPostalCode, "ShipPostalCode");

                entity.HasIndex(e => e.ShippedDate, "ShippedDate");

                entity.HasIndex(e => e.ShipVia, "ShippersOrders");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.CustomerId)
                    .HasMaxLength(5)
                    .IsFixedLength()
                    .HasColumnName("CustomerID");
                entity.Property(e => e.SellerId).HasColumnName("SellerID");
                entity.Property(e => e.Freight)
                    .HasDefaultValue(0m)
                    .HasColumnType("money");
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.RequiredDate).HasColumnType("datetime");
                entity.Property(e => e.ShipAddress).HasMaxLength(60);
                entity.Property(e => e.ShipCity).HasMaxLength(15);
                entity.Property(e => e.ShipCountry).HasMaxLength(15);
                entity.Property(e => e.ShipName).HasMaxLength(40);
                entity.Property(e => e.ShipPostalCode).HasMaxLength(10);
                entity.Property(e => e.ShipRegion).HasMaxLength(15);
                entity.Property(e => e.ShippedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Orders_Customers");

                entity.HasOne(d => d.Seller).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.SellerId)
                    .HasConstraintName("FK_Orders_Sellers");

                entity.HasOne(d => d.ShipViaNavigation).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipVia)
                    .HasConstraintName("FK_Orders_Shippers");
            });

            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId }).HasName("PK_Order_Details");

                entity.ToTable("Order Details");

                entity.HasIndex(e => e.OrderId, "OrderID");

                entity.HasIndex(e => e.OrderId, "OrdersOrder_Details");

                entity.HasIndex(e => e.ProductId, "ProductID");

                entity.HasIndex(e => e.ProductId, "ProductsOrder_Details");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.Quantity).HasDefaultValue((short)1);
                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Orders");

                entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_Details_Products");
            });

            builder.Entity<OrderDetailsExtended>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Order Details Extended");

                entity.Property(e => e.ExtendedPrice).HasColumnType("money");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.UnitPrice).HasColumnType("money");
            });

            builder.Entity<OrderSubtotal>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Order Subtotals");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.Subtotal).HasColumnType("money");
            });

            builder.Entity<OrdersQry>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Orders Qry");

                entity.Property(e => e.Address).HasMaxLength(60);
                entity.Property(e => e.City).HasMaxLength(15);
                entity.Property(e => e.CompanyName).HasMaxLength(40);
                entity.Property(e => e.Country).HasMaxLength(15);
                entity.Property(e => e.CustomerId)
                    .HasMaxLength(5)
                    .IsFixedLength()
                    .HasColumnName("CustomerID");
                entity.Property(e => e.SellerId).HasColumnName("SellerID");
                entity.Property(e => e.Freight).HasColumnType("money");
                entity.Property(e => e.OrderDate).HasColumnType("datetime");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.PostalCode).HasMaxLength(10);
                entity.Property(e => e.Region).HasMaxLength(15);
                entity.Property(e => e.RequiredDate).HasColumnType("datetime");
                entity.Property(e => e.ShipAddress).HasMaxLength(60);
                entity.Property(e => e.ShipCity).HasMaxLength(15);
                entity.Property(e => e.ShipCountry).HasMaxLength(15);
                entity.Property(e => e.ShipName).HasMaxLength(40);
                entity.Property(e => e.ShipPostalCode).HasMaxLength(10);
                entity.Property(e => e.ShipRegion).HasMaxLength(15);
                entity.Property(e => e.ShippedDate).HasColumnType("datetime");
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.CategoryId, "CategoriesProducts");

                entity.HasIndex(e => e.CategoryId, "CategoryID");

                entity.HasIndex(e => e.ProductName, "ProductName");

                entity.HasIndex(e => e.SupplierId, "SupplierID");

                entity.HasIndex(e => e.SupplierId, "SuppliersProducts");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.QuantityPerUnit).HasMaxLength(20);
                entity.Property(e => e.ReorderLevel).HasDefaultValue((short)0);
                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
                entity.Property(e => e.UnitPrice)
                    .HasDefaultValue(0m)
                    .HasColumnType("money");
                entity.Property(e => e.UnitsInStock).HasDefaultValue((short)0);
                entity.Property(e => e.UnitsOnOrder).HasDefaultValue((short)0);

                entity.HasOne(d => d.Category).WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Products_Categories");

                entity.HasOne(d => d.Supplier).WithMany(p => p.Products)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_Products_Suppliers");
            });

            builder.Entity<ProductSalesFor1997>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Product Sales for 1997");

                entity.Property(e => e.CategoryName).HasMaxLength(15);
                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.ProductSales).HasColumnType("money");
            });

            builder.Entity<ProductsAboveAveragePrice>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Products Above Average Price");

                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.UnitPrice).HasColumnType("money");
            });

            builder.Entity<ProductsByCategory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Products by Category");

                entity.Property(e => e.CategoryName).HasMaxLength(15);
                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.QuantityPerUnit).HasMaxLength(20);
            });

            builder.Entity<QuarterlyOrder>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Quarterly Orders");

                entity.Property(e => e.City).HasMaxLength(15);
                entity.Property(e => e.CompanyName).HasMaxLength(40);
                entity.Property(e => e.Country).HasMaxLength(15);
                entity.Property(e => e.CustomerId)
                    .HasMaxLength(5)
                    .IsFixedLength()
                    .HasColumnName("CustomerID");
            });

            builder.Entity<Region>(entity =>
            {
                entity.HasKey(e => e.RegionId).IsClustered(false);

                entity.ToTable("Region");

                entity.Property(e => e.RegionId)
                    .ValueGeneratedNever()
                    .HasColumnName("RegionID");
                entity.Property(e => e.RegionDescription)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            builder.Entity<SalesByCategory>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Sales by Category");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
                entity.Property(e => e.CategoryName).HasMaxLength(15);
                entity.Property(e => e.ProductName).HasMaxLength(40);
                entity.Property(e => e.ProductSales).HasColumnType("money");
            });

            builder.Entity<SalesTotalsByAmount>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Sales Totals by Amount");

                entity.Property(e => e.CompanyName).HasMaxLength(40);
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.SaleAmount).HasColumnType("money");
                entity.Property(e => e.ShippedDate).HasColumnType("datetime");
            });

            builder.Entity<Shipper>(entity =>
            {
                entity.Property(e => e.ShipperId).HasColumnName("ShipperID");
                entity.Property(e => e.CompanyName).HasMaxLength(40);
                entity.Property(e => e.Phone).HasMaxLength(24);
            });

            builder.Entity<SummaryOfSalesByQuarter>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Summary of Sales by Quarter");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ShippedDate).HasColumnType("datetime");
                entity.Property(e => e.Subtotal).HasColumnType("money");
            });

            builder.Entity<SummaryOfSalesByYear>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("Summary of Sales by Year");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ShippedDate).HasColumnType("datetime");
                entity.Property(e => e.Subtotal).HasColumnType("money");
            });

            builder.Entity<Supplier>(entity =>
            {
                entity.HasIndex(e => e.CompanyName, "CompanyName");

                entity.HasIndex(e => e.PostalCode, "PostalCode");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
                entity.Property(e => e.Address).HasMaxLength(60);
                entity.Property(e => e.City).HasMaxLength(15);
                entity.Property(e => e.CompanyName).HasMaxLength(40);
                entity.Property(e => e.ContactName).HasMaxLength(30);
                entity.Property(e => e.ContactTitle).HasMaxLength(30);
                entity.Property(e => e.Country).HasMaxLength(15);
                entity.Property(e => e.Fax).HasMaxLength(24);
                entity.Property(e => e.HomePage).HasColumnType("ntext");
                entity.Property(e => e.Phone).HasMaxLength(24);
                entity.Property(e => e.PostalCode).HasMaxLength(10);
                entity.Property(e => e.Region).HasMaxLength(15);
            });

            builder.Entity<Territory>(entity =>
            {
                entity.HasKey(e => e.TerritoryId).IsClustered(false);

                entity.Property(e => e.TerritoryId)
                    .HasMaxLength(20)
                    .HasColumnName("TerritoryID");
                entity.Property(e => e.RegionId).HasColumnName("RegionID");
                entity.Property(e => e.TerritoryDescription)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.HasOne(d => d.Region).WithMany(p => p.Territories)
                    .HasForeignKey(d => d.RegionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Territories_Region");
            });

        }
    }
}
