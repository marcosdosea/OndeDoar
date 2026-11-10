using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IInstituicaoService
    {
        int Create(Instituicao instituicao);
        void Edit(Instituicao instituicao);
        void Delete(int id);
        Instituicao? Get(int id);
        IEnumerable<Instituicao> GetAll();
        IEnumerable<Instituicao> GetByNome(string nome);
    }
}