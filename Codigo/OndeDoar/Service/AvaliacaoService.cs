using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Service
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly OndeDoarContext context;

        public AvaliacaoService(OndeDoarContext context)
        {
            this.context = context;
        }

        public int Create(Avaliacao avaliacao)
        {
            if (!Validar(avaliacao))
                throw new ArgumentException("Os dados da avaliação são inválidos.");

            context.Add(avaliacao);
            context.SaveChanges();
            return avaliacao.Id;
        }

        public void Delete(int id)
        {
            var avaliacao = context.Avaliacaos.Find(id);
            if (avaliacao != null)
            {
                context.Remove(avaliacao);
                context.SaveChanges();
            }
        }

        public void Edit(Avaliacao avaliacao)
        {
            if (!Validar(avaliacao))
                throw new ArgumentException("Os dados da avaliação são inválidos.");

            context.Update(avaliacao);
            context.SaveChanges();
        }

        public Avaliacao? Get(int id)
        {
            return context.Avaliacaos.Find(id);
        }

        public IEnumerable<Avaliacao> GetAll()
        {
            return context.Avaliacaos.AsNoTracking();
        }

        public bool Validar(Avaliacao avaliacao)
        {
            if (avaliacao.Nota < 0 || avaliacao.Nota > 5) return false;
            if (avaliacao.IdInstituicao <= 0) return false;
            if (avaliacao.IdDoador <= 0) return false;
            if (avaliacao.Data == default) return false;

            return true;
        }
    }
}