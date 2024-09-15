namespace Mango.Web.Models
{
    public class OrderHeaderDto
    {
        public int OrderHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponId { get; set; }
        public double Discount { get; set; }

        public double OrderTotal { get; set; }
        //[Required]
        public string? Name { get; set; }
        //[Required]
        //public string? LastName { get; set; }
        //[Required]
        public string? Phone { get; set; }
        //[Required]
        public string? Email { get; set; }
        public DateTime OrderTime { get; set; }
        public string? Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }
        public IEnumerable<OrderDetailsDto> OrderDetails { get; set; }
    }
}
