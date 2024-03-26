using AutoMapper;
using Northwind.Application.Models.Category;
using Northwind.Application.Models.Employee;
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
                    autoMapperConfig.CreateMap<Employee, EmployeeCreateEditModel>();

                    autoMapperConfig.CreateMap<EmployeeCreateEditModel, Employee>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ImageConverter.ConvertFormFileToByteArray(src.FormFile!)));

                    autoMapperConfig.CreateMap<Employee, EmployeeIndexModel>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ImageConverter.ConvertNorthwindPhoto(src.Photo!, ImageHeaders.Employee)))
                        .ForMember(dest => dest.BirthDate, opts => opts.MapFrom(src => ((DateTime)src.BirthDate!).ToShortDateString()))
                        .ForMember(dest => dest.HireDate, opts => opts.MapFrom(src => ((DateTime)src.HireDate!).ToShortDateString()));

                    autoMapperConfig.CreateMap<EmployeeIndexModel, Employee>();

                    autoMapperConfig.CreateMap<Category, CategoryEditModel>();

                    autoMapperConfig.CreateMap<CategoryEditModel, Category>()
                        .ForMember(dest => dest.Picture, opts => opts.MapFrom(src => ImageConverter.ConvertFormFileToByteArray(src.FormFile!)));

                });

                return config.CreateMapper();
            });
        }
    }
}
