using AutoMapper;
using Northwind.Application.Models.Category;
using Northwind.Application.Models.Employee;
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
                    autoMapperConfig.CreateMap<Employee, EmployeeEditModel>();

                    autoMapperConfig.CreateMap<EmployeeEditModel, Employee>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ImageConverter.ConvertFormFileToByteArray(src.FormFile!)));

                    autoMapperConfig.CreateMap<Category, CategoryEditModel>();

                    autoMapperConfig.CreateMap<CategoryEditModel, Category>()
                        .ForMember(dest => dest.Picture, opts => opts.MapFrom(src => ImageConverter.ConvertFormFileToByteArray(src.FormFile!)));

                });

                return config.CreateMapper();
            });
        }
    }
}
