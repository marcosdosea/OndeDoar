
using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace ServiceTests
{
    [TestClass]
    public class DoadorServiceTests
    {
        private OndeDoarContext _context = null!;
        private IDoadorService _doadorService = null!;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new DbContextOptionsBuilder<OndeDoarContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            var options = builder.Options;
            _context = new InMemoryOndeDoarContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            var doadores = new List<Doador>
            {
                new Doador
                {
                    Id = 1,
                    Nome = "Alice Johnson",
                    Email = "alice@example.com",
                    Telefone = "11987654321"
                },
                new Doador
                {
                    Id = 2,
                    Nome = "Joao Silva",
                    Email = "joao@example.com",
                    Telefone = "21987654321"
                },
                new Doador
                {
                    Id = 3,
                    Nome = "Maria Santos",
                    Email = "maria@example.com",
                    Telefone = "31987654321"
                }
            };

            _context.Doadors.AddRange(doadores);
            _context.SaveChanges();

            _doadorService = new DoadorService(_context);
        }

        [TestMethod]
        public void InserirTest()
        {
            var doador = new Doador
            {
                Id = 4,
                Nome = "Carlos Souza",
                Email = "carlos@example.com",
                Telefone = "11999998888"
            };

            var id = _doadorService.Inserir(doador);

            _context.ChangeTracker.Clear();

            var doadorPersistido = _doadorService.Obter(id);

            Assert.AreEqual(4, _doadorService.ObterTodos().Count());
            Assert.AreEqual(4, id);
            Assert.AreEqual("Carlos Souza", doadorPersistido.Nome);
            Assert.AreEqual("carlos@example.com", doadorPersistido.Email);
            Assert.AreEqual("11999998888", doadorPersistido.Telefone);
        }

        [TestMethod]
        public void RemoverTest()
        {
            const int id = 1;

            _doadorService.Remover(id);
            _context.ChangeTracker.Clear();

            Assert.AreEqual(2, _doadorService.ObterTodos().Count());
            Assert.IsNull(_context.Doadors.Find(id));
        }

        [TestMethod]
        public void EditarTest()
        {
            var doador = _doadorService.Obter(3);

            doador.Nome = "Eduardo Lima";
            doador.Email = "eduardo@example.com";
            doador.Telefone = "11988887777";

            _doadorService.Editar(doador);

            _context.ChangeTracker.Clear();

            var doadorAtualizado = _doadorService.Obter(3);

            Assert.AreEqual("Eduardo Lima", doadorAtualizado.Nome);
            Assert.AreEqual("eduardo@example.com", doadorAtualizado.Email);
            Assert.AreEqual("11988887777", doadorAtualizado.Telefone);
        }

        [TestMethod]
        public void ObterTest()
        {
            const int id = 2;

            var doador = _doadorService.Obter(id);

            Assert.IsNotNull(doador);
            Assert.AreEqual(id, doador.Id);
            Assert.AreEqual("Joao Silva", doador.Nome);
            Assert.AreEqual("joao@example.com", doador.Email);
            Assert.AreEqual("21987654321", doador.Telefone);
        }

        [TestMethod]
        public void ObterTodosTest()
        {
            const int quantidadeEsperada = 3;

            var doadores = _doadorService.ObterTodos()
                .OrderBy(d => d.Id)
                .ToList();

            Assert.AreEqual(quantidadeEsperada, doadores.Count);
            Assert.AreEqual(1, doadores[0].Id);
            Assert.AreEqual("Alice Johnson", doadores[0].Nome);
            Assert.AreEqual(2, doadores[1].Id);
            Assert.AreEqual("Joao Silva", doadores[1].Nome);
            Assert.AreEqual(3, doadores[2].Id);
            Assert.AreEqual("Maria Santos", doadores[2].Nome);
        }

        private sealed class InMemoryOndeDoarContext : OndeDoarContext
        {
            public InMemoryOndeDoarContext(
                DbContextOptions<OndeDoarContext> options)
                : base(options)
            {
            }

            protected override void OnConfiguring(
                DbContextOptionsBuilder optionsBuilder)
            {
            }
        }
    }
}
