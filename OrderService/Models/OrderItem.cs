// Models/OrderItem.cs
namespace OrderService.Models
{
    public class OrderItem
    {
        public int Id { get; set; } // Primary Key
        public int OrderId { get; set; } // FK to Order
        public int ProductId { get; set; } // FK to ProductService
        public int Quantity { get; set; }
        public decimal Price { get; set; } // Price at purchase time
    }
}
