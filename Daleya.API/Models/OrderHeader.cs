using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daleya.API.Models
{
    public class OrderHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateOnly OrderDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public int? CouponId { get; set; }
        public Coupon Coupon { get; set; }
        public decimal Discound { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public TimeOnly OrderTime { get; set; } = TimeOnly.FromDateTime( DateTime.Now);
        public string? Status { get; set; }
        public string? PaymentIntenId { get; set; }
        public string? StripeSessionId { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
