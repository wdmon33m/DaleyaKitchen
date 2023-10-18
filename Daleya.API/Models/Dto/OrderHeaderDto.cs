namespace Daleya.API.Models.Dto
{
    public class OrderHeaderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string? CouponCode { get; set; }
        public CouponDto? Coupon { get; set; }
        public double Discound { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime OrderTime { get; set; } = DateTime.Now;
        public string? Status { get; set; }
        public IEnumerable<OrderDetailsDto>? OrderDetails { get; set; }
    }
}
