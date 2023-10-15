using System.ComponentModel.DataAnnotations;

namespace Daleya.API.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string? CouponCode { get; set; }
        public double Discound { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime OrderTime { get; set; } = DateTime.Now;
        public string? Status { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
