using Mango.Services.Web.Models;
using Mango.Services.Web.Models.Dto;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
	public class OrderService: IOrderService
	{
		private readonly IBaseService _baseService;
		public OrderService(IBaseService baseService)
		{
			_baseService = baseService;
		}
        public async Task<ResponseDto?> CreateOrder(CartDto cartDto)
        {
            try
            {
                return await _baseService.SendAsync(new RequestDto()
                {
                    ApiType = Utility.SD.ApiType.POST,
                    Data = cartDto,
                    Url = SD.OrderAPIBase + "/api/order/CreateOrder/"
                });
            }
            catch (Exception ex) { throw;}
        }
		
		public async Task<ResponseDto?> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            try
            {
                return await _baseService.SendAsync(new RequestDto()
                {
                    ApiType = SD.ApiType.POST,
                    Data = stripeRequestDto,
                    Url = SD.OrderAPIBase + "/api/order/CreateStripeSession"
                });
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseDto?> GetAllOrder(string? userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrders/" + userId,
            });
        }

        public async Task<ResponseDto?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrder/" + orderId,
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string? status)
        {
            try
            {
                return await _baseService.SendAsync(new RequestDto()
                {
                    ApiType = SD.ApiType.POST,
                    Data = status,
                    Url = SD.OrderAPIBase + "/api/order/UpdateOrderStatus/"+ orderId,
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseDto?> ValidateStripeSession(int orderHeaderId)
        {
            try
            {
                return await _baseService.SendAsync(new RequestDto()
                {
                    ApiType = SD.ApiType.POST,
                    Data = orderHeaderId,
                    Url = SD.OrderAPIBase + "/api/order/ValidateStripeSession"
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
