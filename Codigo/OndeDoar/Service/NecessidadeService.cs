using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service
{
    public class NecessidadeService : INecessidadeService
    {
        private static readonly string[] PrioridadesValidas = { "Alta", "Média", "Baixa" };
        private readonly OndeDoarContext _context;

        public NecessidadeService(OndeDoarContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria uma nova necessidade no banco de dados e retorna o id da necessidade criada.
        /// </summary>
        /// <param name="necessidade"> A necessidade a ser criada. </param>
        /// <returns> O id da necessidade criada. </returns>
        /// <exception cref="ServiceException"> Se os dados da necessidade forem inválidos. </exception>
        public int Create(Necessidade necessidade)
        {
            if (!Validar(necessidade))
                throw new ServiceException("Os dados da necessidade são inválidos.", new Exception());

            _context.Add(necessidade);
            _context.SaveChanges();
            return necessidade.Id;
        }

        /// <summary>
        /// Atualiza as informações de uma necessidade existente no banco de dados.
        /// </summary>
        /// <param name="necessidade"> A necessidade a ser atualizada. </param>
        /// <exception cref="ServiceException"> Se os dados da necessidade forem inválidos. </exception>
        public void Edit(Necessidade necessidade)
        {
            if (!Validar(necessidade))
                throw new ServiceException("Os dados da necessidade são inválidos.", new Exception());

            _context.Update(necessidade);
            _context.SaveChanges();
        }

        /// <summary>
        /// Exclui uma necessidade do banco de dados com base no id fornecido.
        /// Se a necessidade não for encontrada, nenhuma ação será realizada.
        /// </summary>
        /// <param name="id"> O id da necessidade a ser excluída. </param>
        public void Delete(int id)
        {
            var necessidade = _context.Necessidades.Find(id);
            if (necessidade != null)
            {
                _context.Remove(necessidade);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Obtém uma necessidade pelo id.
        /// </summary>
        /// <param name="id"> O id da necessidade. </param>
        /// <returns> A necessidade encontrada ou nulo, se ela não existir. </returns>
        public Necessidade? Get(int id)
        {
            return _context.Necessidades.Find(id);
        }

        /// <summary>
        /// Recupera todas as necessidades do banco de dados.
        /// </summary>
        /// <returns> Uma coleção de necessidades. </returns>
        public IEnumerable<Necessidade> GetAll()
        {
            return _context.Necessidades.AsNoTracking();
        }

        /// <summary>
        /// Verifica se os dados da necessidade são válidos: nome e categoria preenchidos,
        /// prioridade Alta, Média ou Baixa e instituição informada.
        /// </summary>
        /// <param name="necessidade"> A necessidade a ser validada. </param>
        /// <returns> Verdadeiro se os dados são válidos; caso contrário, falso. </returns>
        public bool Validar(Necessidade necessidade)
        {
            if (string.IsNullOrWhiteSpace(necessidade.Nome)) return false;
            if (string.IsNullOrWhiteSpace(necessidade.Categoria)) return false;
            if (!PrioridadesValidas.Contains(necessidade.Prioridade)) return false;
            if (necessidade.IdInstituicao <= 0) return false;

            return true;
        }
    }
}