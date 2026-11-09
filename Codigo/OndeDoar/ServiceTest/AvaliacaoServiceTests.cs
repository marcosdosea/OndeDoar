using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class AvaliacaoServiceTests
{
    private OndeDoarContext contexto = null!;
    private IAvaliacaoService servicoAvaliacao = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"OndeDoar_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<OndeDoarContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new OndeDoarContext(opcoes);

        contexto.Avaliacaos.AddRange(
            new Avaliacao
            {
                Id = 1,
                Nota = 5.0f,
                Comentario = "Excelente instituição",
                Data = new DateTime(2026, 10, 1),
                IdInstituicao = 10,
                IdDoador = 100
            },
            new Avaliacao
            {
                Id = 2,
                Nota = 3.5f,
                Comentario = "Bom atendimento",
                Data = new DateTime(2026, 10, 5),
                IdInstituicao = 11,
                IdDoador = 101
            },
            new Avaliacao
            {
                Id = 3,
                Nota = 4.0f,
                Comentario = "Poderia ser melhor",
                Data = new DateTime(2026, 10, 10),
                IdInstituicao = 12,
                IdDoador = 102
            });

        contexto.SaveChanges();

        servicoAvaliacao = new AvaliacaoService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDaAvaliacaoCriada()
    {
        // Arrange
        var avaliacao = new Avaliacao
        {
            Id = 4,
            Nota = 4.5f,
            Comentario = "Ótimo projeto",
            Data = new DateTime(2026, 11, 1),
            IdInstituicao = 13,
            IdDoador = 103
        };

        // Act
        var idRetornado = servicoAvaliacao.Create(avaliacao);

        // Assert
        Assert.AreEqual(4, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarAvaliacao()
    {
        // Arrange
        var avaliacaoEsperada = new Avaliacao
        {
            Id = 4,
            Nota = 4.5f,
            Comentario = "Ótimo projeto",
            Data = new DateTime(2026, 11, 1),
            IdInstituicao = 13,
            IdDoador = 103
        };

        // Act
        servicoAvaliacao.Create(avaliacaoEsperada);

        // Assert
        var avaliacaoCriada = servicoAvaliacao.Get(4);

        Assert.IsNotNull(avaliacaoCriada);
        Assert.AreEqual(avaliacaoEsperada.Id, avaliacaoCriada.Id);
        Assert.AreEqual(avaliacaoEsperada.Nota, avaliacaoCriada.Nota);
        Assert.AreEqual(avaliacaoEsperada.Comentario, avaliacaoCriada.Comentario);
        Assert.AreEqual(avaliacaoEsperada.Data, avaliacaoCriada.Data);
        Assert.AreEqual(avaliacaoEsperada.IdInstituicao, avaliacaoCriada.IdInstituicao);
        Assert.AreEqual(avaliacaoEsperada.IdDoador, avaliacaoCriada.IdDoador);
    }

    [TestMethod]
    public void Delete_QuandoAvaliacaoExiste_DeveRemoverAvaliacao()
    {
        // Act
        servicoAvaliacao.Delete(2);

        // Assert
        var avaliacaoExcluida = servicoAvaliacao.Get(2);
        var avaliacoesRestantes = servicoAvaliacao.GetAll().ToList();

        Assert.IsNull(avaliacaoExcluida);
        Assert.AreEqual(2, avaliacoesRestantes.Count);
        Assert.IsFalse(avaliacoesRestantes.Any(a => a.Id == 2));
    }

    [TestMethod]
    public void Delete_QuandoAvaliacaoNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var avaliacoesIniciais = servicoAvaliacao.GetAll().ToList();

        // Act
        servicoAvaliacao.Delete(999);

        // Assert
        var avaliacoesFinais = servicoAvaliacao.GetAll().ToList();

        Assert.AreEqual(avaliacoesIniciais.Count, avaliacoesFinais.Count);
        CollectionAssert.AreEquivalent(
            avaliacoesIniciais.Select(a => a.Id).ToList(),
            avaliacoesFinais.Select(a => a.Id).ToList());
    }

    [TestMethod]
    public void Edit_QuandoAvaliacaoExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var avaliacao = servicoAvaliacao.Get(3);

        Assert.IsNotNull(avaliacao);

        avaliacao!.Comentario = "Melhorou bastante";
        avaliacao.Nota = 4.8f;

        // Act
        servicoAvaliacao.Edit(avaliacao);

        // Assert
        var avaliacaoAtualizada = servicoAvaliacao.Get(3);

        Assert.IsNotNull(avaliacaoAtualizada);
        Assert.AreEqual("Melhorou bastante", avaliacaoAtualizada.Comentario);
        Assert.AreEqual(4.8f, avaliacaoAtualizada.Nota);
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarAvaliacaoCorrespondente()
    {
        // Act
        var avaliacao = servicoAvaliacao.Get(1);

        // Assert
        Assert.IsNotNull(avaliacao);
        Assert.AreEqual(1, avaliacao.Id);
        Assert.AreEqual(5.0f, avaliacao.Nota);
        Assert.AreEqual("Excelente instituição", avaliacao.Comentario);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var avaliacao = servicoAvaliacao.Get(999);

        // Assert
        Assert.IsNull(avaliacao);
    }

    [TestMethod]
    public void GetAll_QuandoExistemAvaliacoes_DeveRetornarTodasAsAvaliacoes()
    {
        // Act
        var avaliacoes = servicoAvaliacao.GetAll().ToList();

        // Assert
        Assert.IsNotNull(avaliacoes);
        Assert.AreEqual(3, avaliacoes.Count);

        CollectionAssert.AreEquivalent(
            new int[] { 1, 2, 3 },
            avaliacoes.Select(a => a.Id).ToList());
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaAvaliacoes_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Avaliacaos.RemoveRange(contexto.Avaliacaos);
        contexto.SaveChanges();

        // Act
        var avaliacoes = servicoAvaliacao.GetAll().ToList();

        // Assert
        Assert.IsNotNull(avaliacoes);
        Assert.AreEqual(0, avaliacoes.Count);
    }

    [TestMethod]
    public void Validar_QuandoDadosSaoValidos_DeveRetornarVerdadeiro()
    {
        // Arrange
        var avaliacaoValida = new Avaliacao
        {
            Nota = 4.0f,
            Data = DateTime.Now,
            IdInstituicao = 1,
            IdDoador = 1
        };

        // Act
        var resultado = servicoAvaliacao.Validar(avaliacaoValida);

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Validar_QuandoNotaInvalida_DeveRetornarFalso()
    {
        // Arrange
        var avaliacaoInvalida = new Avaliacao
        {
            Nota = 6.0f, // Fora do limite 0 a 5
            Data = DateTime.Now,
            IdInstituicao = 1,
            IdDoador = 1
        };

        // Act
        var resultado = servicoAvaliacao.Validar(avaliacaoInvalida);

        // Assert
        Assert.IsFalse(resultado);
    }
}