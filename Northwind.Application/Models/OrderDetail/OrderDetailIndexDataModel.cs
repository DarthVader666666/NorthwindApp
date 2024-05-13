using Northwind.Data.Entities;

namespace Northwind.Application.Models.OrderDetail
{
    public class OrderDetailIndexDataModel
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public decimal TotalPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }

        public string? ProductName { get; set; }
    }
}
