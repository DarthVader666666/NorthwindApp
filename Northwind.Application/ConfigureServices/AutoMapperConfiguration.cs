using AutoMapper;
using Northwind.Data.Dtos;
using Northwind.Data.Entities;

namespace NorthwindApp.ConfigureServices
{
    public static class AutomapperConfiguration
    {
        private static byte[] OleHeader = new byte[] { 21, 28, 47, 0, 2, 0, 0, 0, 13, 0, 14, 0, 20,
            0, 33, 0, 255, 255, 255, 255, 66, 105, 116, 109, 97, 112, 32, 73, 109, 97, 103, 101, 0, 80, 97,
            105, 110, 116, 46, 80, 105, 99, 116, 117, 114, 101, 0, 1, 5, 0, 0, 2, 0, 0, 0, 7, 0, 0, 0, 80,
            66, 114, 117, 115, 104, 0, 0, 0, 0, 0, 0, 0, 0, 0, 160, 41, 0, 0 };

        private static bool HasHeader(this byte[] source, byte[] header)
        {
            if (source.Length < header.Length)
            {
                return false;
            }

            for (int i = 0; i < header.Length; i++)
            {
                if (source[i] != header[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static byte[] ConvertNorthwindPhoto(byte[] source) =>
            source.HasHeader(OleHeader) ? source[OleHeader.Length..] : source;

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var config = new MapperConfiguration(autoMapperConfig =>
                {
                    autoMapperConfig.CreateMap<Employee, EmployeeDto>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ConvertNorthwindPhoto(src.Photo!)));

                    autoMapperConfig.CreateMap<EmployeeDto, Employee>()
                        .ForMember(dest => dest.Photo, opts => opts.MapFrom(src => ConvertNorthwindPhoto(src.Photo!)));
                });

                return config.CreateMapper();
            });
        }
    }
}
