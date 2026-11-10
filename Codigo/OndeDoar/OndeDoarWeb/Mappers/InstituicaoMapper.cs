using Core;
using Mapster;
using OndeDoarWeb.Models;

namespace OndeDoarWeb.Mapper
{
    public class InstituicaoMapper
    {
        public static InstituicaoViewModel ToViewModel(Instituicao instituicao)
        {
            return instituicao.Adapt<InstituicaoViewModel>();
        }

        public static Instituicao ToEntity(InstituicaoViewModel instituicaoViewModel)
        {
            return instituicaoViewModel.Adapt<Instituicao>();
        }

        public static IEnumerable<InstituicaoViewModel> ToViewModel(IEnumerable<Instituicao> instituicoes)
        {
            return instituicoes.Adapt<IEnumerable<InstituicaoViewModel>>();
        }

        public static IEnumerable<Instituicao> ToEntity(IEnumerable<InstituicaoViewModel> instituicoesViewModel)
        {
            return instituicoesViewModel.Adapt<IEnumerable<Instituicao>>();
        }
    }
}