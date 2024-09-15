using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.EmailAPI.Models.Dto
{
    public class CartHeaderDto
    {
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponId { get; set; }
        public double Discount { get; set; }
     
        public double CartTotal { get; set; }
        public string? Name { get; set; }
        //public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

    }
}
