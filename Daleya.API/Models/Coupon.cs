using System.ComponentModel.DataAnnotations;

namespace Daleya.API.Models
{
    public class Coupon
    {
        [Required]
        public int CouponId { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public decimal DiscountAmount { get; set; }
        [Required]
        public decimal MinAmount { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
