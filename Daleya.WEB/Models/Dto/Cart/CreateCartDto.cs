namespace Daleya.WEB.Models.Dto.Cart
{
    public class CreateCartDto
    {
        public CreateCartHeaderDto CartHeader { get; set; }
        public IEnumerable<CreateCartDetailsDto>? CartDetails { get; set; }
    }
}
