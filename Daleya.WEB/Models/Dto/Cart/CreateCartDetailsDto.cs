namespace Daleya.WEB.Models.Dto.Cart
{
    public class CreateCartDetailsDto
    {
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
    }
}
