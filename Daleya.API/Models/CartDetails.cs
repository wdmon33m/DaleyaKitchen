using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daleya.API.Models
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsId { get; set; }

        public int CartHeaderId { get; set; }
        [NotMapped]
        [ForeignKey(nameof(CartHeaderId))]
        public CartHeader? CartHeader { get; set; }

        public int ProductId { get; set; }
        [NotMapped]
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        public int Count {get; set; }
    }
}
