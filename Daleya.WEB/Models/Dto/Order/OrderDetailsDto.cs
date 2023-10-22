namespace Daleya.WEB.Models.Dto.Order
{
    public class OrderDetailsDto
    {
        public int OrderDetailId { get; set; }

        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public ProductDto Product { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }

        public int OrderId { get; set; }
        public OrderHeaderDto OrderHeader { get; set; }
    }
}
