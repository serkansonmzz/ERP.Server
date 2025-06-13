using AutoMapper;
using ERP.Server.Application.Common.Dtos;
using ERP.Server.Domain.Entities;

namespace ERP.Server.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Name));
    }
}
