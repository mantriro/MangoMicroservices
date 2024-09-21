using Mango.Services.Web.Models;
using Mango.Services.Web.Models.Dto;
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface IOrderService
	{
		Task<ResponseDto?> CreateOrder(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeReqDto);
        Task<ResponseDto?> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDto?> GetAllOrder(string? userId);
        Task<ResponseDto?> GetOrder(int orderId);
        Task<ResponseDto?> UpdateOrderStatus(int orderId, string? status);

    }
}
