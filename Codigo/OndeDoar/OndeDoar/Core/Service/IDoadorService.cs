namespace Core.Service
{
    public interface IDoadorService
    {
        void Editar(Doador doador);
        int Inserir(Doador doador);
        Doador Obter(int id);
        IEnumerable<Doador> ObterTodos();
        void Remover(int id);
    }
}
