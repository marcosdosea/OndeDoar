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
        /// Edita os dados da necessidade no banco de dados.
        /// </summary>
        /// <param name="necessidade">Dados da necessidade.</param>
        /// <exception cref="ServiceException">Prioridade inválida.</exception>
        public void Editar(Necessidade necessidade)
        {
            Validar(necessidade);
            _context.Update(necessidade);
            _context.SaveChanges();
        }

        /// <summary>
        /// Insere uma nova necessidade no banco de dados.
        /// </summary>
        /// <param name="necessidade">Dados da necessidade.</param>
        /// <returns>Identificador da necessidade inserida.</returns>
        /// <exception cref="ServiceException">Prioridade inválida.</exception>
        public int Inserir(Necessidade necessidade)
        {
            Validar(necessidade);
            _context.Necessidades.Add(necessidade);
            _context.SaveChanges();
            return necessidade.Id;
        }

        /// <summary>
        /// Busca a necessidade com o identificador especificado.
        /// </summary>
        /// <param name="id">Identificador da necessidade.</param>
        /// <returns>A necessidade encontrada.</returns>
        /// <exception cref="ServiceException">Necessidade não encontrada.</exception>
        public Necessidade Obter(int id)
        {
            Necessidade? necessidade = _context.Necessidades.Find(id);
            if (necessidade == null)
            {
                throw new ServiceException($"Necessidade com id {id} não encontrada.", new Exception());
            }
            return necessidade;
        }

        /// <summary>
        /// Obtém todas as necessidades.
        /// </summary>
        /// <returns>Coleção com todas as necessidades.</returns>
        public IEnumerable<Necessidade> ObterTodos()
        {
            return _context.Necessidades.AsNoTracking().ToList();
        }

        /// <summary>
        /// Remove a necessidade com o identificador especificado.
        /// </summary>
        /// <param name="id">Identificador da necessidade.</param>
        /// <exception cref="ServiceException">Necessidade não encontrada.</exception>
        public void Remover(int id)
        {
            Necessidade necessidade = Obter(id);
            _context.Necessidades.Remove(necessidade);
            _context.SaveChanges();
        }

        /// <summary>
        /// Regra de negócio: a prioridade deve ser Alta, Média ou Baixa.
        /// </summary>
        private static void Validar(Necessidade necessidade)
        {
            if (!PrioridadesValidas.Contains(necessidade.Prioridade))
            {
                throw new ServiceException("Prioridade inválida. Use Alta, Média ou Baixa.", new Exception());
            }
        }
    }
}