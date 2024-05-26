using AutoMapper;
using Northwind.Application.Constants;
using Northwind.Application.Models.Category;
using Northwind.Application.Models.Customer;
using Northwind.Application.Models.Employee;
using Northwind.Application.Models.Order;
using Northwind.Application.Models.OrderDetail;
using Northwind.Application.Models.Product;
using Northwind.Application.Models.Supplier;
using Northwind.Bll.Services;
using Northwind.Data.Entities;

namespace NorthwindApp.ConfigureServices
{
    public static class AutomapperConfiguration
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            _ = services.AddSingleton(provider =>
            {
                var config = new MapperConfiguration(autoMapperConfig =>
                {
                    autoMapperConfig.CreateMap<Employee, EmployeeEditModel>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(
                            src => ImageConverter.ConvertNorthwindPhoto(src.Photo!)));

                    autoMapperConfig.CreateMap<EmployeeEditModel, Employee>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(
                            src => src.FormFile != null && src.FormFile.Length > 0 ? ImageConverter.ConvertFormFileToByteArray(src.FormFile!) : src.Photo))
                        .ForMember(dest => dest.ReportsTo, opts => opts.MapFrom(
                            src => src.ReportsTo == 0 ? null : src.ReportsTo));

                    autoMapperConfig.CreateMap<Employee, EmployeeIndexModel>()
                        .ForMember(dest => dest.FullName, opts => opts.MapFrom(src => src.FirstName + " " + src.LastName))
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ImageConverter.ConvertNorthwindPhoto(src.Photo!)));

                    autoMapperConfig.CreateMap<EmployeeIndexModel, Employee>();

                    autoMapperConfig.CreateMap<EmployeeCreateModel, Employee>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ImageConverter.ConvertFormFileToByteArray(src.FormFile!)))
                        .ForMember(dest => dest.ReportsTo, opts => opts.MapFrom(src => src.ReportsTo == 0 ? null : src.ReportsTo));

                    autoMapperConfig.CreateMap<Employee, EmployeeDetailsModel>();



                    autoMapperConfig.CreateMap<Category, CategoryIndexModel>()
                        .ForMember(dest => dest.Picture, opts => opts.MapFrom(src => ImageConverter.ConvertNorthwindPhoto(src.Picture!)));

                    autoMapperConfig.CreateMap<Category, CategoryDetailsModel>()
                        .ForMember(dest => dest.Picture, opts => opts.MapFrom(src => ImageConverter.ConvertNorthwindPhoto(src.Picture!)));

                    autoMapperConfig.CreateMap<Category, CategoryEditModel>()
                        .ForMember(dest => dest.Picture, opts => opts.MapFrom(
                            src => ImageConverter.ConvertNorthwindPhoto(src.Picture!)));

                    autoMapperConfig.CreateMap<CategoryEditModel, Category>()
                        .ForMember(dest => dest.Picture, opts => opts.MapFrom(
                            src => src.FormFile != null ? ImageConverter.ConvertFormFileToByteArray(src.FormFile!) : src.Picture));

                    autoMapperConfig.CreateMap<CategoryIndexModel, Category>();

                    autoMapperConfig.CreateMap<CategoryCreateModel, Category>()
                        .ForMember(dest => dest.Picture, opts => opts.MapFrom(src => ImageConverter.ConvertFormFileToByteArray(src.FormFile!)));



                    autoMapperConfig.CreateMap<Product, ProductIndexDataModel>();
                    autoMapperConfig.CreateMap<Product, ProductDetailsModel>();

                    autoMapperConfig.CreateMap<ProductCreateModel, Product>()
                        .ForMember(dest => dest.CategoryId, opts => opts.MapFrom(src => src.CategoryId == 0 ? null : src.CategoryId))
                        .ForMember(dest => dest.SupplierId, opts => opts.MapFrom(src => src.SupplierId == 0 ? null : src.SupplierId));

                    autoMapperConfig.CreateMap<Product, ProductDeleteModel>();

                    autoMapperConfig.CreateMap<Product, ProductEditModel>()
                        .ForMember(dest => dest.CategoryId, opts => opts.MapFrom(src => src.CategoryId))
                        .ForMember(dest => dest.SupplierId, opts => opts.MapFrom(src => src.SupplierId));

                    autoMapperConfig.CreateMap<ProductEditModel, Product>()
                        .ForMember(dest => dest.CategoryId, opts => opts.MapFrom(src => src.CategoryId == 0 ? null : src.CategoryId))
                        .ForMember(dest => dest.SupplierId, opts => opts.MapFrom(src => src.SupplierId == 0 ? null : src.SupplierId))
                        .ForMember(dest => dest.Category, opts => opts.Ignore())
                        .ForMember(dest => dest.Supplier, opts => opts.Ignore());



                    autoMapperConfig.CreateMap<Customer, CustomerIndexDataModel>();
                    autoMapperConfig.CreateMap<CustomerCreateModel, Customer>()
                        .ForMember(dest => dest.CustomerId, opts => opts.MapFrom(src => src.CustomerId.ToUpper()));

                    autoMapperConfig.CreateMap<Customer, CustomerDeleteModel>();
                    autoMapperConfig.CreateMap<Customer, CustomerDetailsModel>();
                    autoMapperConfig.CreateMap<CustomerEditModel, Customer>();
                    autoMapperConfig.CreateMap<Customer, CustomerEditModel>();



                    autoMapperConfig.CreateMap<Order, OrderIndexDataModel>()
                        .ForMember(dest => dest.TotalPrice, opts => opts.MapFrom(
                            src => src.OrderDetails.Select(x => x.Quantity * x.UnitPrice - x.Quantity * x.UnitPrice * (decimal)x.Discount).Sum()))
                        .ForMember(dest => dest.OrderStatus, opts => opts.MapFrom(src => GetStatus(src)));

                    autoMapperConfig.CreateMap<Order, OrderWorkflowModel>()
                        .ForMember(dest => dest.OrderStatus, opts => opts.MapFrom(src => GetStatus(src)))
                        .ForMember(dest => dest.OrderNumber, opts => opts.MapFrom(src => src.OrderId));

                    autoMapperConfig.CreateMap<Order, OrderDetailsModel>()
                        .ForMember(dest => dest.ShipperId, opts => opts.MapFrom(src => src.ShipVia));

                    autoMapperConfig.CreateMap<Order, OrderEditModel>()
                        .ForMember(dest => dest.ShipperId, opts => opts.MapFrom(src => src.ShipVia))
                        .ForMember(dest => dest.CustomerIdList, opts => opts.Ignore())
                        .ForMember(dest => dest.EmployeeIdList, opts => opts.Ignore());

                    autoMapperConfig.CreateMap<Order, OrderCreateModel>()
                        .ForMember(dest => dest.ShipperId, opts => opts.MapFrom(src => src.ShipVia));

                    autoMapperConfig.CreateMap<OrderDetailsModel, Order>()
                        .ForMember(dest => dest.ShipVia, opts => opts.MapFrom(src => src.ShipperId));

                    autoMapperConfig.CreateMap<OrderEditModel, Order>()
                        .ForMember(dest => dest.ShipVia, opts => opts.MapFrom(src => src.ShipperId == 0 ? null : src.ShipperId))
                        .ForMember(dest => dest.EmployeeId, opts => opts.MapFrom(src => src.EmployeeId == 0 ? null : src.EmployeeId))
                        .ForMember(dest => dest.CustomerId, opts => opts.MapFrom(src => src.CustomerId == "" ? null : src.CustomerId))
                        .ForMember(dest => dest.Customer, opts => opts.Ignore())
                        .ForMember(dest => dest.Employee, opts => opts.Ignore());

                    autoMapperConfig.CreateMap<OrderCreateModel, Order>()
                        .ForMember(dest => dest.ShipVia, opts => opts.MapFrom(src => src.ShipperId == 0 ? null : src.ShipperId))
                        .ForMember(dest => dest.EmployeeId, opts => opts.MapFrom(src => src.EmployeeId == 0 ? null : src.EmployeeId))
                        .ForMember(dest => dest.CustomerId, opts => opts.MapFrom(src => src.CustomerId == "" ? null : src.CustomerId))
                        .ForMember(dest => dest.OrderDate, opts => opts.MapFrom(src => DateTime.UtcNow));


                    autoMapperConfig.CreateMap<OrderDetailIndexDataModel, OrderDetail>()
                        .ForMember(dest => dest.Discount, opts => opts.MapFrom(src => Math.Round(src.Discount / 100, 2)));

                    autoMapperConfig.CreateMap<OrderDetail, OrderDetailIndexDataModel>()
                        .ForMember(dest => dest.ProductName, opts => opts.MapFrom(src => src.Product.ProductName))
                        .ForMember(dest => dest.Discount, opts => opts.MapFrom(src => src.Discount * 100))
                        .ForMember(dest => dest.Price, opts => opts.MapFrom(src => src.Quantity * src.UnitPrice - (src.Quantity * src.UnitPrice) * (decimal)src.Discount));

                    autoMapperConfig.CreateMap<OrderDetailCreateModel, OrderDetail>()
                        .ForMember(dest => dest.Discount, opts => opts.MapFrom(src => Math.Round((float)src.Discount / 100, 2)));



                    autoMapperConfig.CreateMap<Supplier, SupplierIndexDataModel>();
                    autoMapperConfig.CreateMap<SupplierCreateModel, Supplier>();
                    autoMapperConfig.CreateMap<Supplier, SupplierDeleteModel>();
                    autoMapperConfig.CreateMap<Supplier, SupplierDetailsModel>();
                    autoMapperConfig.CreateMap<SupplierEditModel, Supplier>();
                    autoMapperConfig.CreateMap<Supplier, SupplierEditModel>();
                });

                return config.CreateMapper();
            });
        }

        private static string? GetStatus(Order? order)
        { 
            return order switch
                {
                    Order x when x.OrderDate != null && x.RequiredDate == null && x.ShippedDate == null => SessionValues.Confirmed,
                    Order x when x.OrderDate != null && x.RequiredDate != null && x.ShippedDate == null => SessionValues.InProgress,
                    Order x when x.OrderDate != null && x.RequiredDate != null && x.ShippedDate != null => SessionValues.Completed,
                    _ => SessionValues.NotConfirmed,
                };
        }
    }
}
