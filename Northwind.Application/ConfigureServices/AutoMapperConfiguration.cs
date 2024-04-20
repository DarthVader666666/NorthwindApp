﻿using AutoMapper;
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
                            src => src.FormFile != null ? ImageConverter.ConvertFormFileToByteArray(src.FormFile!) : src.Photo));

                    autoMapperConfig.CreateMap<Employee, EmployeeIndexModel>()
                        .ForMember(dest => dest.FullName, opts => opts.MapFrom(src => src.FirstName + " " + src.LastName))
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ImageConverter.ConvertNorthwindPhoto(src.Photo!)));                        

                    autoMapperConfig.CreateMap<EmployeeIndexModel, Employee>();

                    autoMapperConfig.CreateMap<EmployeeCreateModel, Employee>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ImageConverter.ConvertFormFileToByteArray(src.FormFile!)));



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



                    autoMapperConfig.CreateMap<Product, ProductIndexModel>();
                    autoMapperConfig.CreateMap<ProductCreateModel, Product>();

                    autoMapperConfig.CreateMap<Product, ProductEditModel>()
                        .ForMember(dest => dest.SupplierName, opts => opts.MapFrom(
                            src => src.Supplier == null ? "" : src.Supplier.CompanyName ));

                    autoMapperConfig.CreateMap<ProductEditModel, Product>();
                });

                return config.CreateMapper();
            });
        }
    }
}
