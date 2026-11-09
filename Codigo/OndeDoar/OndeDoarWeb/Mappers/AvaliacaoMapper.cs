using Core;
using Mapster;
using OndeDoarWeb.Models;

namespace OndeDoarWeb.Mapper
{
    public class AvaliacaoMapper
    {
        public static AvaliacaoViewModel ToViewModel(Avaliacao avaliacao)
        {
            return avaliacao.Adapt<AvaliacaoViewModel>();
        }

        public static Avaliacao ToEntity(AvaliacaoViewModel avaliacaoViewModel)
        {
            return avaliacaoViewModel.Adapt<Avaliacao>();
        }

        public static IEnumerable<AvaliacaoViewModel> ToViewModel(IEnumerable<Avaliacao> avaliacoes)
        {
            return avaliacoes.Adapt<IEnumerable<AvaliacaoViewModel>>();
        }

        public static IEnumerable<Avaliacao> ToEntity(IEnumerable<AvaliacaoViewModel> avaliacoesViewModel)
        {
            return avaliacoesViewModel.Adapt<IEnumerable<Avaliacao>>();
        }
    }
}