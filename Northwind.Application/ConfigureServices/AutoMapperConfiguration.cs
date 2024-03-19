using AutoMapper;
using Northwind.Application.Models.Employee;
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
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ConvertFormFileToByteArray(src.FormFile!)));
                });

                return config.CreateMapper();
            });
        }

        private static byte[]? ConvertFormFileToByteArray(IFormFile formFile)
        {
            if (formFile == null)
            {
                return null;
            }

            using var reader = new BinaryReader(formFile.OpenReadStream());
            return reader.ReadBytes((int)formFile.Length);
        }
    }
}
