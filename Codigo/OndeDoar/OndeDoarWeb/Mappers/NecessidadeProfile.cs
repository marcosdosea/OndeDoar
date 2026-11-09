using AutoMapper;
using Core;
using Models;

namespace Mappers
{
    public class NecessidadeProfile : Profile
    {
        public NecessidadeProfile()
        {
            // Na entidade, Status é byte[] (1 byte). No model, vira bool (Ativa).
            CreateMap<Necessidade, NecessidadeModel>()
                .ForMember(m => m.Ativa,
                    opt => opt.MapFrom(n => n.Status != null && n.Status.Length > 0 && n.Status[0] != 0));

            CreateMap<NecessidadeModel, Necessidade>()
                .ForMember(n => n.Status,
                    opt => opt.MapFrom(m => new byte[] { (byte)(m.Ativa ? 1 : 0) }));
        }
    }
}