using Microsoft.AspNetCore.Mvc.Rendering;
using Northwind.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Application.Models.OrderDetail
{
    public class OrderDetailEditModel
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public decimal UnitPrice { get; set; }

        public short Quantity { get; set; }

        public float Discount { get; set; }
    }
}
