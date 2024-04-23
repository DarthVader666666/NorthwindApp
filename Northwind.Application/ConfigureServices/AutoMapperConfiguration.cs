using AutoMapper;
using Northwind.Application.Models.Category;
using Northwind.Application.Models.Employee;
using Northwind.Application.Models.Product;
using Northwind.Bll.Enums;
using Northwind.Bll.Services;
using Northwind.Data.Entities;

namespace NorthwindApp.ConfigureServices
{
    public static class AutomapperConfiguration
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
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
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ImageConverter.ConvertFormFileToByteArray(src.FormFile!)));

                    autoMapperConfig.CreateMap<Employee, EmployeeDetailsModel>();



                    autoMapperConfig.CreateMap<Category, CategoryIndexModel>()
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



                    autoMapperConfig.CreateMap<Product, ProductIndexModel>()
                        .ForMember(dest => dest.UnitPrice, opts => opts.MapFrom(
                            src => string.Format("{0:f2}", src.UnitPrice)));

                    autoMapperConfig.CreateMap<Product, ProductDetailsModel>()
                        .ForMember(dest => dest.UnitPrice, opts => opts.MapFrom(src => string.Format("{0:f2}", src.UnitPrice)));

                    autoMapperConfig.CreateMap<ProductCreateModel, Product>();

                    autoMapperConfig.CreateMap<Product, ProductEditModel>();

                    autoMapperConfig.CreateMap<ProductEditModel, Product>();
                });

                return config.CreateMapper();
            });
        }
    }
}
