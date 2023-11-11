using AutoMapper;
using Voltage.Entities.Models.Dtos;

namespace Voltage.Helper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Entities.Entity.Message, MessageDto>();   
    }
}
