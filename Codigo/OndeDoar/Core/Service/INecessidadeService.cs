namespace Core.Service
{
    public interface INecessidadeService
    {
        int Create(Necessidade necessidade);
        void Edit(Necessidade necessidade);
        void Delete(int id);
        Necessidade? Get(int id);
        IEnumerable<Necessidade> GetAll();
        bool Validar(Necessidade necessidade);
    }
}
