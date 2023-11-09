using AutoMapper;
using Voltage.Entities.Models;

namespace Voltage.Helper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Entities.Entity.Message, MessageDto>();   
    }
}
