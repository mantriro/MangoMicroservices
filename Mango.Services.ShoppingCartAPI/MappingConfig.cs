using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig= new MapperConfiguration(config =>
			{
				config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
				config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
//                config.CreateMap<CartDetails, CartDetailsDto>();
//                config.CreateMap<CartHeader, CartHeaderDto>();

            });
			return mappingConfig;
		}
	}
}
