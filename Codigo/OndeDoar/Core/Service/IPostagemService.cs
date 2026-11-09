using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service
{
    public interface IPostagemService
    {
        uint Create(Postagem postagem);
        void Edit(Postagem postagem);
        void Delete(uint id);
        Postagem? Get(uint id);
        IEnumerable<Postagem> GetAll();
        IEnumerable<Postagem> GetByTitulo(string titulo);

    }
}
