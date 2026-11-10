using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public class InstituicaoService : IInstituicaoService
    {
        private readonly OndeDoarContext _context;

        public InstituicaoService(OndeDoarContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cadastra uma nova instituição, verificando se o e-mail já não está em uso.
        /// </summary>
        public int Create(Instituicao instituicao)
        {
            ValidarEmailEmUso(instituicao);
            _context.Add(instituicao);
            _context.SaveChanges();
            return instituicao.Id;
        }

        /// <summary>
        /// Atualiza os dados da instituição, mantendo a regra de e-mail único.
        /// </summary>
        public void Edit(Instituicao instituicao)
        {
            ValidarEmailEmUso(instituicao);
            _context.Update(instituicao);
            _context.SaveChanges();
        }

        /// <summary>
        /// Remove a instituição com o identificador informado.
        /// </summary>
        public void Delete(int id)
        {
            var instituicao = _context.Instituicaos.Find(id);
            if (instituicao == null)
                return;

            try
            {
                _context.Remove(instituicao);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new ServiceException(
                    "Não foi possível excluir a instituição porque ela possui registros vinculados (necessidades, postagens, avaliações ou notificações).",
                    ex);
            }
        }

        /// <summary>
        /// Busca a instituição pelo identificador. Retorna null se não existir.
        /// </summary>
        public Instituicao? Get(int id)
        {
            return _context.Instituicaos.AsNoTracking().FirstOrDefault(i => i.Id == id);
        }

        /// <summary>
        /// Obtém todas as instituições.
        /// </summary>
        public IEnumerable<Instituicao> GetAll()
        {
            return _context.Instituicaos.AsNoTracking().ToList();
        }

        /// <summary>
        /// Busca por nome fantasia ou razão social.
        /// </summary>
        public IEnumerable<Instituicao> GetByNome(string nome)
        {
            return _context.Instituicaos
                .AsNoTracking()
                .Where(i => i.NomeFantasia.Contains(nome) || i.RazaoSocial.Contains(nome))
                .ToList();
        }

        private void ValidarEmailEmUso(Instituicao instituicao)
        {
            bool emUso = _context.Instituicaos
                .AsNoTracking()
                .Any(i => i.Email == instituicao.Email && i.Id != instituicao.Id);

            if (emUso)
                throw new ServiceException("O e-mail informado já está em uso por outra instituição.");
        }
    }
}