using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class InstituicaoServiceTests
{
    private OndeDoarContext contexto = null!;
    private IInstituicaoService servicoInstituicao = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"OndeDoar_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<OndeDoarContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new OndeDoarContext(opcoes);

        contexto.Instituicaos.AddRange(
            CriarInstituicao(1, "Lar Esperança", "Lar Esperança LTDA", "lar@esperanca.org"),
            CriarInstituicao(2, "Casa Solidária", "Casa Solidária ONG", "casa@solidaria.org"),
            CriarInstituicao(3, "Abrigo Amigo", "Abrigo Amigo Associação", "abrigo@amigo.org")
        );
        contexto.SaveChanges();
        contexto.ChangeTracker.Clear();

        servicoInstituicao = new InstituicaoService(contexto);
    }

    [TestCleanup]
    public void Limpar()
    {
        contexto.Dispose();
    }

    private static Instituicao CriarInstituicao(int id, string nomeFantasia, string razaoSocial, string email)
    {
        return new Instituicao
        {
            Id = id,
            NomeFantasia = nomeFantasia,
            RazaoSocial = razaoSocial,
            Cnpj = "12345678000199",
            Cep = "49500-000",
            Cidade = "Itabaiana",
            Estado = "SE",
            Telefone = "79999999999",
            Email = email,
            HoraAbertura = new TimeOnly(8, 0),
            HoraFechamento = new TimeOnly(17, 0)
        };
    }

    [TestMethod]
    public void Create_DeveAdicionarInstituicaoERetornarId()
    {
        var nova = CriarInstituicao(0, "Nova Casa", "Nova Casa LTDA", "nova@casa.org");

        var id = servicoInstituicao.Create(nova);

        Assert.HasCount(4, servicoInstituicao.GetAll());
        Assert.IsGreaterThan(0, id);
        var salva = servicoInstituicao.Get(id);
        Assert.IsNotNull(salva);
        Assert.AreEqual("Nova Casa", salva.NomeFantasia);
    }

    [TestMethod]
    public void Create_QuandoEmailJaEmUso_DeveLancarServiceException()
    {
        var duplicada = CriarInstituicao(0, "Outra", "Outra LTDA", "lar@esperanca.org");

        Assert.Throws<ServiceException>(() => servicoInstituicao.Create(duplicada));
        Assert.HasCount(3, servicoInstituicao.GetAll());
    }

    [TestMethod]
    public void Edit_DeveAtualizarDadosDaInstituicao()
    {
        var existente = servicoInstituicao.Get(1)!;
        existente.NomeFantasia = "Lar Esperança Atualizado";

        servicoInstituicao.Edit(existente);

        Assert.AreEqual("Lar Esperança Atualizado", servicoInstituicao.Get(1)!.NomeFantasia);
    }

    [TestMethod]
    public void Edit_MantendoOMesmoEmail_NaoDeveLancarExcecao()
    {
        var existente = servicoInstituicao.Get(2)!;
        existente.Cidade = "Aracaju";

        servicoInstituicao.Edit(existente);

        Assert.AreEqual("Aracaju", servicoInstituicao.Get(2)!.Cidade);
    }

    [TestMethod]
    public void Edit_QuandoEmailPertenceAOutraInstituicao_DeveLancarServiceException()
    {
        var existente = servicoInstituicao.Get(2)!;
        existente.Email = "lar@esperanca.org";

        Assert.Throws<ServiceException>(() => servicoInstituicao.Edit(existente));
    }

    [TestMethod]
    public void Delete_QuandoExiste_DeveRemoverInstituicao()
    {
        servicoInstituicao.Delete(3);

        Assert.HasCount(2, servicoInstituicao.GetAll());
        Assert.IsNull(servicoInstituicao.Get(3));
    }

    [TestMethod]
    public void Delete_QuandoNaoExiste_NaoDeveAlterarOsDados()
    {
        servicoInstituicao.Delete(99);

        Assert.HasCount(3, servicoInstituicao.GetAll());
    }

    [TestMethod]
    public void Get_QuandoExiste_DeveRetornarInstituicao()
    {
        var instituicao = servicoInstituicao.Get(2);

        Assert.IsNotNull(instituicao);
        Assert.AreEqual("Casa Solidária", instituicao.NomeFantasia);
    }

    [TestMethod]
    public void Get_QuandoNaoExiste_DeveRetornarNull()
    {
        Assert.IsNull(servicoInstituicao.Get(99));
    }

    [TestMethod]
    public void GetAll_DeveRetornarTodasAsInstituicoes()
    {
        var lista = servicoInstituicao.GetAll();

        Assert.HasCount(3, lista);
    }

    [TestMethod]
    public void GetByNome_DeveBuscarPorNomeFantasia()
    {
        var lista = servicoInstituicao.GetByNome("Solidária");

        Assert.HasCount(1, lista);
        Assert.AreEqual(2, lista.First().Id);
    }

    [TestMethod]
    public void GetByNome_DeveBuscarPorRazaoSocial()
    {
        var lista = servicoInstituicao.GetByNome("Associação");

        Assert.HasCount(1, lista);
        Assert.AreEqual(3, lista.First().Id);
    }

    [TestMethod]
    public void GetByNome_QuandoNaoEncontra_DeveRetornarListaVazia()
    {
        Assert.IsEmpty(servicoInstituicao.GetByNome("Inexistente"));
    }
}