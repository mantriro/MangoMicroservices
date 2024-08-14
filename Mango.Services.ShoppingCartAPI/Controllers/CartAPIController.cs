using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;
        public CartAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

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
                var cartHeaderFromDb = _db.CartHeaders.FirstOrDefault(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                   CartHeader cartHeader= _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    _db.SaveChangesAsync();
                     cartDto.CartDetails.First().CartHeaderId= cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails.FirstOrDefaultAsync(
                       u => u.ProductId == cartDto.CartDetails.First().ProductId 
                       && 
                       u.CartHeaderId== cartHeaderFromDb.CartHeaderId);
                    if(cartDetailsFromDb == null)
                    {
                        //create cartdetails

                    }
                    else
                    {
                        //update count in cart details
                    }
                }
            }
            catch(Exception ex) 
            {
                _response.Message= ex.Message.ToString();
                _response.Success = false;
            }
            finally
            {

            }
                

        
        }
    }
}
