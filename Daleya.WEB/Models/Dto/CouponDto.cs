namespace Daleya.WEB.Models.Dto
{
    public class CouponDto
    {
        public int CouponID { get; set; }
        public string CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal MinAmount { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
