using AutoMapper;
using Azure;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]

    public class ProductAPIController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;
        public ProductAPIController(AppDbContext db, IMapper mapper)
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
                IEnumerable<Product> objList = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<Product>>(objList);
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
                Product obj = _db.Products.First(x => x.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(obj);
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
        [Route("GetByName/{code}")]
        public ResponseDto GetByName(string name)
        {
            try
            {
                Product obj = _db.Products.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
                if (obj == null)
                {
                    _response.Success = false;
                }
                _response.Result = _mapper.Map<ProductDto>(obj);
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
        //[Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto productDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(productDto); // convert dto to coupon
                _db.Products.Add(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(obj); // map obj to response
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        //[Authorize(Roles = "ADMIN")]

        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(productDto); // convert dto to coupon
                _db.Products.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(obj); // map obj to response
            }
            catch (Exception ex)
            {
                _response.Success = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        //[Authorize(Roles = "ADMIN")]

        public ResponseDto Delete(int id)
        {
            try
            {
                Product obj = _db.Products.First(x => x.ProductId == id); // convert dto to coupon
                _db.Products.Remove(obj);
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
