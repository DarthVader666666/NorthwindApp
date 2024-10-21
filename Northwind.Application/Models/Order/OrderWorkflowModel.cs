using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northwind.Application.Models.Order
{
    public class OrderWorkflowModel
    {
        public int? OrderNumber { get; set; }

        public int? SellerId { get; set; }

        public string? OrderStatus { get; set; }

        public SelectList? SellerList { get; set; }
    }
}
