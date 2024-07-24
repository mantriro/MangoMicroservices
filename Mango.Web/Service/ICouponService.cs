﻿using Mango.Services.Web.Models;
using Mango.Web.Models;

namespace Mango.Web.Service
{
	public interface ICouponService
	{
		Task<ResponseDto?> GetCouponAsync(string couponCode);
		Task<ResponseDto?> GetAllCouponAsync();
		Task<ResponseDto?> GetCouponByIdAsync(int id);
		Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto);		
		Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
		Task<ResponseDto?> DeleteCouponsAsync(int id);

	}
}
