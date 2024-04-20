using Microsoft.AspNetCore.Http;
using Northwind.Bll.Enums;

namespace Northwind.Bll.Services
{
    public static class ImageConverter
    {
        private static byte[] OleEmployeeHeader = { 21, 28, 47, 0, 2, 0, 0, 0, 13, 0, 14, 0, 20,
            0, 33, 0, 255, 255, 255, 255, 66, 105, 116, 109, 97, 112, 32, 73, 109, 97, 103, 101, 0, 80, 97,
            105, 110, 116, 46, 80, 105, 99, 116, 117, 114, 101, 0, 1, 5, 0, 0, 2, 0, 0, 0, 7, 0, 0, 0, 80,
            66, 114, 117, 115, 104, 0, 0, 0, 0, 0, 0, 0, 0, 0, 32, 84, 0, 0 };

        private static byte[] OleCategoryHeader = { 21, 28, 47, 0, 2, 0, 0, 0, 13, 0, 14, 0, 20,
            0, 33, 0, 255, 255, 255, 255, 66, 105, 116, 109, 97, 112, 32, 73, 109, 97, 103, 101, 0, 80, 97,
            105, 110, 116, 46, 80, 105, 99, 116, 117, 114, 101, 0, 1, 5, 0, 0, 2, 0, 0, 0, 7, 0, 0, 0, 80,
            66, 114, 117, 115, 104, 0, 0, 0, 0, 0, 0, 0, 0, 0, 160, 41, 0, 0 };

        private const int headerLength = 78;

        public static byte[]? ConvertFormFileToByteArray(IFormFile formFile)
        {
            if (formFile == null)
            {
                return null;
            }

            using var reader = new BinaryReader(formFile.OpenReadStream());
            return reader.ReadBytes((int)formFile.Length);
        }

        public static byte[] ConvertNorthwindPhoto(byte[] source)
        {
            if (source.Length >= headerLength && (source[..headerLength].SequenceEqual(OleEmployeeHeader) || source[..headerLength].SequenceEqual(OleCategoryHeader)))
            {
                return source[headerLength..];
            }

            return source;
        }
    }
}
