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

	}
}
