using System.ComponentModel.DataAnnotations;

namespace Daleya.API.Models
{
    public class Coupon
    {
        [Required]
        public int CouponID { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        [Required]
        public double MinAmount { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
