using System;
using System.Collections.Generic;

namespace Core.Service
{
    public interface IAvaliacaoService
    {
        int Create(Avaliacao avaliacao);
        void Edit(Avaliacao avaliacao);
        void Delete(int id);
        Avaliacao? Get(int id);
        IEnumerable<Avaliacao> GetAll();
        bool Validar(Avaliacao avaliacao);
    }
}