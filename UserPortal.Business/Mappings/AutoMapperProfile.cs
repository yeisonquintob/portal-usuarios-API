// Ruta: ./UserPortal.Business/Mappings/AutoMapperProfile.cs
using AutoMapper;
using UserPortal.Data.Entities;
using UserPortal.Shared.DTOs.Response;
using UserPortal.Shared.Models;

namespace UserPortal.Business.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserResponseDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role!.Name))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastLogin, opt => opt.MapFrom(src => src.LastLogin));

        CreateMap<User, UserModel>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role!.Name));

        // Mapeos para resultados paginados
        CreateMap(typeof(PaginatedResult<>), typeof(PaginatedResult<>));
    }
}