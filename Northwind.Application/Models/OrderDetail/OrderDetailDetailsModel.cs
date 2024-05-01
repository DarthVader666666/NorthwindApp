using Northwind.Data.Entities;

namespace Northwind.Application.Models.OrderDetail
{
    public class OrderDetailDetailsModel
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }
    }
}
