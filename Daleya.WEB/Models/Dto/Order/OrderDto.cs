namespace Daleya.WEB.Models.Dto.Order
{
    public class OrderDto
    {
        public OrderHeaderDto OrderHeader { get; set; }
        public IEnumerable<OrderDetailsDto>? OrderDetails { get; set; }
    }
}
