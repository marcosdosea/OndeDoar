using AutoMapper;
using Models;
using Core;

namespace Mappers
{
    public class DoadorProfile : Profile
    {
        public DoadorProfile()
        {
            CreateMap<Doador, DoadorModel>().ReverseMap();
        }
    }
}
