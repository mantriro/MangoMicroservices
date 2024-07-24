using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
	[Route("api/[Controller]")]
	[ApiController]
	public class CouponAPIController : Controller
	{
		private readonly AppDbContext _db;
		private readonly ResponseDto _response;
		private IMapper _mapper;
		public CouponAPIController(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
			_response = new ResponseDto();
		}
		[HttpGet]
		public object Get()
		{
			try
			{
				IEnumerable<Coupon> objList = _db.Coupons.ToList();
				_response.Result = _mapper.Map<IEnumerable<Coupon>>(objList);
				return _response;
			}
			catch (Exception ex) 
			{
				_response.Success = false;
				_response.Message = ex.Message;
				
			}
			return null;
		}

		[HttpGet]
		[Route("{id:int}")]
		public ResponseDto Get(int id)
		{
			try
			{
				Coupon obj = _db.Coupons.First(x => x.CouponId == id);
				_response.Result = _mapper.Map<CouponDto>(obj);
				return _response;
			}
			catch (Exception ex)
			{
				_response.Success = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpGet]
		[Route("GetByCode/{code}")]
		public ResponseDto GetByCode(string code)
		{
			try
			{
				Coupon obj = _db.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == code.ToLower());
				if (obj == null)
				{
					_response.Success = false;
				}
				_response.Result = _mapper.Map<CouponDto>(obj);
				return _response;
			}
			catch (Exception ex)
			{
				_response.Success = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpPost]
		public ResponseDto Post([FromBody] CouponDto couponDto)
		{
			try
			{
				Coupon obj = _mapper.Map<Coupon>(couponDto); // convert dto to coupon
				_db.Coupons.Add(obj);
				_db.SaveChanges();

				_response.Result= _mapper.Map<CouponDto>(obj); // map obj to response
			}
			catch (Exception ex)
			{
				_response.Success = false;
				_response.Message = ex.Message;
			}
			return _response;
		}


		[HttpPut]
		public ResponseDto Put([FromBody] CouponDto couponDto)
		{
			try
			{
				Coupon obj = _mapper.Map<Coupon>(couponDto); // convert dto to coupon
				_db.Coupons.Update(obj);
				_db.SaveChanges();

				_response.Result = _mapper.Map<CouponDto>(obj); // map obj to response
			}
			catch (Exception ex)
			{
				_response.Success = false;
				_response.Message = ex.Message;
			}
			return _response;
		}

		[HttpDelete]
		public ResponseDto Delete(int id)
		{
			try
			{
				Coupon obj = _db.Coupons.First(x=>x.CouponId==id); // convert dto to coupon
				_db.Coupons.Remove(obj);
				_db.SaveChanges();
			}
			catch (Exception ex)
			{
				_response.Success = false;
				_response.Message = ex.Message;
			}
			return _response;
		}



	}
}
