using Core;
using Core.Service;

namespace Service
{
    class DoadorService : IDoadorService
    {
        private readonly OndeDoarContext _context;
        public DoadorService(OndeDoarContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Edita dados do doador no banco de dados.
        /// </summary>
        /// <param name="doador">Dados do doador.</param>
        public void Editar(Doador doador)
        {
            _context.Update(doador);
            _context.SaveChanges();
        }

        /// <summary>
        /// Insere um novo doador no banco de dados.
        /// </summary>
        /// <param name="doador">Dados do doador.</param>
        /// <returns>Indentificador do doador inserido.</returns>
        public int Inserir(Doador doador)
        {
            _context.Doadors.Add(doador);
            _context.SaveChanges();
            return doador.Id;
        }

        /// <summary>
        /// Busca o doador com o identificador especificado.
        /// </summary>
        /// <param name="id">O identificador do doador para recuperar.</param>
        /// <returns>O doador com o identificador especificado, ou null se nenhum doador correspondente for encontrado.</returns>
        public Doador Obter(int id)
        {
            Doador doador = _context.Doadors.Find(id);
            if(doador == null)
            {
                throw new ServiceException($"Doador com id {id} não encontrado.", new Exception());
            }
            return doador;
        }

        /// <summary>
        /// Obtém todos os doadores.
        /// </summary>
        /// <returns>Uma coleção enumerável com todos os doadores.</returns>
        public IEnumerable<Doador> ObterTodos()
        {
            return _context.Doadors.ToList();
        }

        /// <summary>
        /// Remove o doador com o identificador especificado e salva as alterações.
        /// </summary>
        /// <param name="id">O identificador do doador para remover.</param>
        public void Remover(int id)
        {
            var _doador = _context.Doadors.Find(id);
            _context.Doadors.Remove(_doador);
            _context.SaveChanges();
        }
    }
}
