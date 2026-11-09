using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Service
{
    public class PostagemService : IPostagemService
    {
        private readonly OndeDoarContext context;

        public PostagemService(OndeDoarContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Cria um novo postagem no banco de dados e retorna o id do postagem criado.
        /// </summary>
        /// <param name="postagem"> O postagem a ser criado. </param>
        /// <returns> O id do postagem criado. </returns>
        public uint Create(Postagem postagem)
        {
            context.Add(postagem);
            context.SaveChanges();
            return postagem.Id;
        }

        /// <summary>
        /// Exclui um postagem do banco de dados com base no id fornecido. Se o postagem não for encontrado, nenhuma ação será realizada.
        /// </summary>
        /// <param name="id"> O id do postagem a ser excluído. </param>
        public void Delete(uint id)
        {
            var postagem = context.Postagems.Find(id);
            if (postagem != null)
            {
                context.Remove(postagem);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Atualiza as informações de um postagem existente no banco de dados. Se o postagem não for encontrado, nenhuma ação será realizada.    
        /// </summary>
        /// <param name="postagem"> O postagem a ser atualizado. </param>
        public void Edit(Postagem postagem)
        {
            context.Update(postagem);
            context.SaveChanges();
        }

        /// <summary>
        /// Obtém um postagem pelo id.
        /// </summary>
        /// <param name="id">id do postagem</param>
        /// <returns></returns>
        public Postagem? Get(uint id)
        {
            return context.Postagems.Find(id);
        }

        /// <summary>
        /// Recupera todos os postagemes do banco de dados.
        /// </summary>
        /// <returns>uma coleção de postagemes</returns>
        public IEnumerable<Postagem> GetAll()
        {
            return context.Postagems.AsNoTracking();
        }

        public IEnumerable<Postagem> GetByTitulo(string titulo)
        {
            return context.Postagems.Where(a => a.Titulo.StartsWith(titulo)).AsNoTracking();
        }
    }
}
