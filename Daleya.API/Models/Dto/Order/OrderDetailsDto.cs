namespace Daleya.API.Models.Dto.Order
{
    public class OrderDetailsDto
    {
        public int OrderDetailId { get; set; }

        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public int OrderId { get; set; }
    }
}
