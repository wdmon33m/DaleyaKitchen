namespace Daleya.API.Models.Dto.Order
{
    public class OrderDto
    {
        public OrderHeaderDto OrderHeader { get; set; }
        public IEnumerable<OrderDetailsDto>? OrderDetails { get; set; }
    }
}
