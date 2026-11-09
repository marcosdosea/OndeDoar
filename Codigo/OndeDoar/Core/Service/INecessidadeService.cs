namespace Core.Service
{
    public interface INecessidadeService
    {
        void Editar(Necessidade necessidade);
        int Inserir(Necessidade necessidade);
        Necessidade Obter(int id);
        IEnumerable<Necessidade> ObterTodos();
        void Remover(int id);
    }
}
