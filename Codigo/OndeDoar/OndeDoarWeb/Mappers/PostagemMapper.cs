using Core;
using Mapster;
using OndeDoarWeb.Models;

namespace OndeDoarWeb.Mapper
{
    public class PostagemMapper
    {
        public static PostagemViewModel ToViewModel(Postagem postagem)
        {
            return postagem.Adapt<PostagemViewModel>();
        }

        public static Postagem ToEntity(PostagemViewModel postagemViewModel)
        {
            return postagemViewModel.Adapt<Postagem>();
        }

        public static IEnumerable<PostagemViewModel> ToViewModel(IEnumerable<Postagem> postagens)
        {
            return postagens.Adapt<IEnumerable<PostagemViewModel>>();
        }

        public static IEnumerable<Postagem> ToEntity(IEnumerable<PostagemViewModel> postagensViewModel)
        {
            return postagensViewModel.Adapt<IEnumerable<Postagem>>();
        }
    }
}