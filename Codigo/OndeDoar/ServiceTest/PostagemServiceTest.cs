using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class PostagemServiceTests
{
    private OndeDoarContext contexto = null!;
    private IPostagemService servicoPostagem = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"OndeDoar_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<OndeDoarContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new OndeDoarContext(opcoes);

        contexto.Postagems.AddRange(
            new Postagem
            {
                Id = 1,
                Titulo = "Campanha de Alimentos",
                Descricao = "Arrecadação de cestas básicas",
                Data = new DateTime(2026, 10, 1),
                InstituicaoId = 1
            },
            new Postagem
            {
                Id = 2,
                Titulo = "Campanha de Agasalho",
                Descricao = "Arrecadação de cobertores e casacos",
                Data = new DateTime(2026, 6, 1),
                InstituicaoId = 1
            },
            new Postagem
            {
                Id = 3,
                Titulo = "Doação de Livros",
                Descricao = "Arrecadação de livros infantis",
                Data = new DateTime(2026, 5, 15),
                InstituicaoId = 2
            });

        contexto.SaveChanges();

        servicoPostagem = new PostagemService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDaPostagemCriada()
    {
        // Arrange
        var postagem = new Postagem
        {
            Id = 4,
            Titulo = "Doação de Brinquedos",
            Descricao = "Arrecadação para o Dia das Crianças",
            Data = new DateTime(2026, 10, 12),
            InstituicaoId = 3
        };

        // Act
        var idRetornado = servicoPostagem.Create(postagem);

        // Assert
        Assert.AreEqual(4u, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarPostagem()
    {
        // Arrange
        var postagemEsperada = new Postagem
        {
            Id = 4,
            Titulo = "Doação de Brinquedos",
            Descricao = "Arrecadação para o Dia das Crianças",
            Data = new DateTime(2026, 10, 12),
            InstituicaoId = 3
        };

        // Act
        servicoPostagem.Create(postagemEsperada);

        // Assert
        var postagemCriada = servicoPostagem.Get(4);

        Assert.IsNotNull(postagemCriada);
        Assert.AreEqual(postagemEsperada.Id, postagemCriada.Id);
        Assert.AreEqual(postagemEsperada.Titulo, postagemCriada.Titulo);
        Assert.AreEqual(postagemEsperada.Descricao, postagemCriada.Descricao);
        Assert.AreEqual(postagemEsperada.Data, postagemCriada.Data);
        Assert.AreEqual(postagemEsperada.InstituicaoId, postagemCriada.InstituicaoId);
    }

    [TestMethod]
    public void Delete_QuandoPostagemExiste_DeveRemoverPostagem()
    {
        // Act
        servicoPostagem.Delete(2);

        // Assert
        var postagemExcluida = servicoPostagem.Get(2);
        var postagensRestantes = servicoPostagem.GetAll().ToList();

        Assert.IsNull(postagemExcluida);
        Assert.HasCount(2, postagensRestantes);
        Assert.IsFalse(postagensRestantes.Any(postagem => postagem.Id == 2));
    }

    [TestMethod]
    public void Delete_QuandoPostagemNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var postagensIniciais = servicoPostagem.GetAll().ToList();

        // Act
        servicoPostagem.Delete(999);

        // Assert
        var postagensFinais = servicoPostagem.GetAll().ToList();

        Assert.HasCount(postagensIniciais.Count, postagensFinais);
        CollectionAssert.AreEquivalent(
            postagensIniciais.Select(postagem => postagem.Id).ToList(),
            postagensFinais.Select(postagem => postagem.Id).ToList());
    }

    [TestMethod]
    public void Edit_QuandoPostagemExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var postagem = servicoPostagem.Get(3);

        Assert.IsNotNull(postagem);

        postagem!.Titulo = "Doação de Material Escolar";
        postagem.Descricao = "Arrecadação de mochilas e cadernos";

        // Act
        servicoPostagem.Edit(postagem);

        // Assert
        var postagemAtualizada = servicoPostagem.Get(3);

        Assert.IsNotNull(postagemAtualizada);
        Assert.AreEqual("Doação de Material Escolar", postagemAtualizada.Titulo);
        Assert.AreEqual("Arrecadação de mochilas e cadernos", postagemAtualizada.Descricao);
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarPostagemCorrespondente()
    {
        // Act
        var postagem = servicoPostagem.Get(1);

        // Assert
        Assert.IsNotNull(postagem);
        Assert.AreEqual(1u, postagem.Id);
        Assert.AreEqual("Campanha de Alimentos", postagem.Titulo);
        Assert.AreEqual("Arrecadação de cestas básicas", postagem.Descricao);
        Assert.AreEqual(1, postagem.InstituicaoId);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var postagem = servicoPostagem.Get(999);

        // Assert
        Assert.IsNull(postagem);
    }

    [TestMethod]
    public void GetAll_QuandoExistemPostagens_DeveRetornarTodasAsPostagens()
    {
        // Act
        var postagens = servicoPostagem.GetAll().ToList();

        // Assert
        Assert.IsNotNull(postagens);
        Assert.HasCount(3, postagens);

        CollectionAssert.AreEquivalent(
            new uint[] { 1, 2, 3 },
            postagens.Select(postagem => postagem.Id).ToList());
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaPostagens_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Postagems.RemoveRange(contexto.Postagems);
        contexto.SaveChanges();

        // Act
        var postagens = servicoPostagem.GetAll().ToList();

        // Assert
        Assert.IsNotNull(postagens);
        Assert.IsEmpty(postagens);
    }

    [TestMethod]
    public void GetByTitulo_QuandoExisteCorrespondencia_DeveRetornarPostagensCorrespondentes()
    {
        // Act
        var postagens = servicoPostagem.GetByTitulo("Campanha").ToList();

        // Assert
        Assert.HasCount(2, postagens);
        Assert.IsTrue(postagens.All(postagem => postagem.Titulo.StartsWith("Campanha")));
    }

    [TestMethod]
    public void GetByTitulo_QuandoTituloCorrespondeAoInicio_DeveRetornarTodasAsPostagensCorrespondentes()
    {
        // Arrange
        contexto.Postagems.Add(new Postagem
        {
            Id = 4,
            Titulo = "Campanha de Sangue",
            Descricao = "Doação de sangue no hemocentro",
            Data = new DateTime(2026, 11, 1),
            InstituicaoId = 2
        });

        contexto.SaveChanges();

        // Act
        var postagens = servicoPostagem.GetByTitulo("Campanha").ToList();

        // Assert
        Assert.HasCount(3, postagens);
        Assert.IsTrue(
            postagens.All(postagem => postagem.Titulo.StartsWith("Campanha")));
    }

    [TestMethod]
    public void GetByTitulo_QuandoNaoExisteCorrespondencia_DeveRetornarColecaoVazia()
    {
        // Act
        var postagens = servicoPostagem.GetByTitulo("Inexistente").ToList();

        // Assert
        Assert.IsNotNull(postagens);
        Assert.IsEmpty(postagens);
    }

    [TestMethod]
    public void GetByTitulo_QuandoTituloEstaVazio_DeveRetornarTodasAsPostagens()
    {
        // Act
        var postagens = servicoPostagem.GetByTitulo(string.Empty).ToList();

        // Assert
        Assert.HasCount(3, postagens);
    }
}