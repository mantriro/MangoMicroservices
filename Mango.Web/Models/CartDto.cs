using Mango.Services.Web.Models.Dto;

namespace Mango.Services.Web.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; }
        public IEnumerable<CartDetailsDto> CartDetails { get; set; } = new List<CartDetailsDto>();
    }
}
