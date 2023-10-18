namespace Daleya.API.Models.Dto
{
    public class Cart
    {
        public CartHeader CartHeader { get; set; }
        public IEnumerable<CartDetails>? CartDetails { get; set; }
    }
}
