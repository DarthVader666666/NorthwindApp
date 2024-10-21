namespace Northwind.Application.Models.Seller
{
    public class SellerIndexModel
    {
        public int SellerId { get; set; }

        public string FullName { get; set; } = null!;

        public string? Title { get; set; }

        public byte[]? Photo { get; set; }
    }
}
