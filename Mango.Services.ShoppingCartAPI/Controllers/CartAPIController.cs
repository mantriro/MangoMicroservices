﻿using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public CartAPIController(AppDbContext db, IMapper mapper, IProductService productService, ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }


        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
          {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userId))

                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.
                    Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product= productDtos.FirstOrDefault(u=>u.ProductId== item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }
                //apply coupon logic if any

                if (!string.IsNullOrEmpty(cart.CartHeader.CouponId))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponId);
                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }
                _response.Result = cart;
            }
            catch (Exception ex) 
            { 
                _response.Success = false;
                _response.Message=ex.Message;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponId = cartDto.CartHeader.CouponId;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;


            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        //[HttpPost("RemoveCoupon")]
        //public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        //{
        //    try
        //    {
        //        var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
        //        cartFromDb.CouponId = "";
        //        _db.CartHeaders.Update(cartFromDb);
        //        await _db.SaveChangesAsync();
        //        _response.Result = true;


        //    }
        //    catch (Exception ex)
        //    {
        //        _response.Success = false;
        //        _response.Message = ex.Message;
        //    }
        //    return _response;
        //}


        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            // User adds Item to cart-
            //    Create CartHeader and CartDetails
            //User adds new item into cart with existing items-
            //    Find Cart Header and add cart details to same header
            //User updates quantity of existing item in cart-
            //    Find details and update count details in cart
            //3 POINTS ABOVE ME ARE IMPLEMENTED IN THE 3 IF ELSE ELSE LOOPS BELOW ME burra vadu
            try
            {
                var cartHeaderFromDb = _db.CartHeaders.AsNoTracking().FirstOrDefault(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and cart details

                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                       u => u.ProductId == cartDto.CartDetails.First().ProductId
                       &&
                       u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;

                        cartDto.CartDetails.First().CartHeaderId += cartDetailsFromDb.CartHeaderId;

                        cartDto.CartDetails.First().CartDetailsId += cartDetailsFromDb.CartDetailsId;

                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.Success = false;
            }
            return _response;



        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cartDetailsId)
        {
            // User adds Item to cart-
            //    Create CartHeader and CartDetails
            //User adds new item into cart with existing items-
            //    Find Cart Header and add cart details to same header
            //User updates quantity of existing item in cart-
            //    Find details and update count details in cart
            //3 POINTS ABOVE ME ARE IMPLEMENTED IN THE 3 IF ELSE ELSE LOOPS BELOW ME burra vadu
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

                int totalCountCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.Success = false;
            }
            return _response;



        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {

                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));

                _response.Result = true;


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
