using Core;
using Mapster;
using Models;

namespace Mappers
{
    public static class NecessidadeMapper
    {
        static NecessidadeMapper()
        {
            // Na entidade, Status é byte[] (1 byte). No model, vira bool (Ativa).
            TypeAdapterConfig<Necessidade, NecessidadeViewModel>.NewConfig()
                .Map(dest => dest.Ativa,
                     src => src.Status != null && src.Status.Length > 0 && src.Status[0] != 0);

            TypeAdapterConfig<NecessidadeViewModel, Necessidade>.NewConfig()
                .Map(dest => dest.Status,
                     src => new byte[] { (byte)(src.Ativa ? 1 : 0) });
        }

        public static NecessidadeViewModel ToViewModel(Necessidade necessidade)
        {
            return necessidade.Adapt<NecessidadeViewModel>();
        }

        public static List<NecessidadeViewModel> ToViewModel(IEnumerable<Necessidade> necessidades)
        {
            return necessidades.Adapt<List<NecessidadeViewModel>>();
        }

        public static Necessidade ToEntity(NecessidadeViewModel necessidadeModel)
        {
            return necessidadeModel.Adapt<Necessidade>();
        }
    }
}