using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daleya.API.Models
{
    public class CartHeader
    {
        [Key]
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        [NotMapped]
        public decimal Discound { get; set; }
        [NotMapped]
        public decimal CartTotal { get; set;}
    }
}
