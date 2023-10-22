namespace Daleya.API.Models.Dto.Order
{
    public class OrderHeaderDto
    {
        public int OrderId { get; set; }
        public string? UserId { get; set; }
        public DateOnly OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
        public string? CouponCode { get; set; }
        public CouponDto? Coupon { get; set; }
        public decimal Discound { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public TimeOnly OrderTime { get; set; }
        public string? Status { get; set; }
        public string? PaymentIntenId { get; set; }
        public string? StripeSessionId { get; set; }
        public IEnumerable<OrderDetailsDto>? OrderDetails { get; set; }
    }
}
