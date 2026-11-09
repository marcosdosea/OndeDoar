using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;
using Service;

namespace Service.Tests;

[TestClass]
public class NecessidadeServiceTests
{
    private OndeDoarContext contexto = null!;
    private INecessidadeService servicoNecessidade = null!;

    [TestInitialize]
    public void Inicializar()
    {
        var nomeBanco = $"OndeDoar_{Guid.NewGuid()}";

        var opcoes = new DbContextOptionsBuilder<OndeDoarContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        contexto = new OndeDoarContext(opcoes);

        contexto.Necessidades.AddRange(
            new Necessidade
            {
                Id = 1,
                Nome = "Cestas básicas",
                Categoria = "Alimentos",
                Descricao = "Alimentos não perecíveis",
                Prioridade = "Alta",
                IdInstituicao = 10,
                Status = new byte[] { 1 }
            },
            new Necessidade
            {
                Id = 2,
                Nome = "Roupas infantis",
                Categoria = "Roupas",
                Descricao = "Roupas para crianças de 4 a 12 anos",
                Prioridade = "Média",
                IdInstituicao = 10,
                Status = new byte[] { 1 }
            },
            new Necessidade
            {
                Id = 3,
                Nome = "Produtos de higiene",
                Categoria = "Higiene",
                Descricao = "Sabonetes e shampoos",
                Prioridade = "Baixa",
                IdInstituicao = 11,
                Status = new byte[] { 0 }
            });

        contexto.SaveChanges();

        servicoNecessidade = new NecessidadeService(contexto);
    }

    [TestCleanup]
    public void Finalizar()
    {
        contexto.Dispose();
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveRetornarIdDaNecessidadeCriada()
    {
        // Arrange
        var necessidade = new Necessidade
        {
            Id = 4,
            Nome = "Fraldas infantis",
            Categoria = "Saúde",
            Descricao = "Fraldas tamanho M",
            Prioridade = "Média",
            IdInstituicao = 10,
            Status = new byte[] { 1 }
        };

        // Act
        var idRetornado = servicoNecessidade.Create(necessidade);

        // Assert
        Assert.AreEqual(4, idRetornado);
    }

    [TestMethod]
    public void Create_QuandoDadosSaoValidos_DeveArmazenarNecessidade()
    {
        // Arrange
        var necessidadeEsperada = new Necessidade
        {
            Id = 4,
            Nome = "Fraldas infantis",
            Categoria = "Saúde",
            Descricao = "Fraldas tamanho M",
            Prioridade = "Média",
            IdInstituicao = 10,
            Status = new byte[] { 1 }
        };

        // Act
        servicoNecessidade.Create(necessidadeEsperada);

        // Assert
        var necessidadeCriada = servicoNecessidade.Get(4);

        Assert.IsNotNull(necessidadeCriada);
        Assert.AreEqual(necessidadeEsperada.Nome, necessidadeCriada.Nome);
        Assert.AreEqual(necessidadeEsperada.Categoria, necessidadeCriada.Categoria);
        Assert.AreEqual(necessidadeEsperada.Descricao, necessidadeCriada.Descricao);
        Assert.AreEqual(necessidadeEsperada.Prioridade, necessidadeCriada.Prioridade);
        Assert.AreEqual(necessidadeEsperada.IdInstituicao, necessidadeCriada.IdInstituicao);
    }

    [TestMethod]
    public void Create_QuandoPrioridadeInvalida_DeveLancarServiceException()
    {
        // Arrange
        var necessidade = new Necessidade
        {
            Id = 4,
            Nome = "Fraldas infantis",
            Categoria = "Saúde",
            Prioridade = "Urgente",
            IdInstituicao = 10
        };

        // Act e Assert
        Assert.Throws<ServiceException>(() => servicoNecessidade.Create(necessidade));
        Assert.AreEqual(3, servicoNecessidade.GetAll().Count());
    }

    [TestMethod]
    public void Delete_QuandoNecessidadeExiste_DeveRemoverNecessidade()
    {
        // Act
        servicoNecessidade.Delete(2);

        // Assert
        var necessidadeExcluida = servicoNecessidade.Get(2);
        var necessidadesRestantes = servicoNecessidade.GetAll().ToList();

        Assert.IsNull(necessidadeExcluida);
        Assert.AreEqual(2, necessidadesRestantes.Count);
        Assert.IsFalse(necessidadesRestantes.Any(n => n.Id == 2));
    }

    [TestMethod]
    public void Delete_QuandoNecessidadeNaoExiste_NaoDeveAlterarOsDados()
    {
        // Arrange
        var necessidadesIniciais = servicoNecessidade.GetAll().ToList();

        // Act
        servicoNecessidade.Delete(999);

        // Assert
        var necessidadesFinais = servicoNecessidade.GetAll().ToList();

        Assert.AreEqual(necessidadesIniciais.Count, necessidadesFinais.Count);
        CollectionAssert.AreEquivalent(
            necessidadesIniciais.Select(n => n.Id).ToList(),
            necessidadesFinais.Select(n => n.Id).ToList());
    }

    [TestMethod]
    public void Edit_QuandoNecessidadeExiste_DeveAtualizarSeusDados()
    {
        // Arrange
        var necessidade = servicoNecessidade.Get(3);

        Assert.IsNotNull(necessidade);

        necessidade!.Descricao = "Sabonetes, shampoos e escovas de dente";
        necessidade.Prioridade = "Alta";

        // Act
        servicoNecessidade.Edit(necessidade);

        // Assert
        var necessidadeAtualizada = servicoNecessidade.Get(3);

        Assert.IsNotNull(necessidadeAtualizada);
        Assert.AreEqual("Sabonetes, shampoos e escovas de dente", necessidadeAtualizada.Descricao);
        Assert.AreEqual("Alta", necessidadeAtualizada.Prioridade);
    }

    [TestMethod]
    public void Edit_QuandoDadosSaoInvalidos_DeveLancarServiceException()
    {
        // Arrange
        var necessidade = servicoNecessidade.Get(1);

        Assert.IsNotNull(necessidade);

        necessidade!.Nome = "";

        // Act e Assert
        Assert.Throws<ServiceException>(() => servicoNecessidade.Edit(necessidade));
    }

    [TestMethod]
    public void Get_QuandoIdExiste_DeveRetornarNecessidadeCorrespondente()
    {
        // Act
        var necessidade = servicoNecessidade.Get(1);

        // Assert
        Assert.IsNotNull(necessidade);
        Assert.AreEqual(1, necessidade.Id);
        Assert.AreEqual("Cestas básicas", necessidade.Nome);
        Assert.AreEqual("Alta", necessidade.Prioridade);
    }

    [TestMethod]
    public void Get_QuandoIdNaoExiste_DeveRetornarNulo()
    {
        // Act
        var necessidade = servicoNecessidade.Get(999);

        // Assert
        Assert.IsNull(necessidade);
    }

    [TestMethod]
    public void GetAll_QuandoExistemNecessidades_DeveRetornarTodasAsNecessidades()
    {
        // Act
        var necessidades = servicoNecessidade.GetAll().ToList();

        // Assert
        Assert.IsNotNull(necessidades);
        Assert.AreEqual(3, necessidades.Count);

        CollectionAssert.AreEquivalent(
            new int[] { 1, 2, 3 },
            necessidades.Select(n => n.Id).ToList());
    }

    [TestMethod]
    public void GetAll_QuandoNaoHaNecessidades_DeveRetornarColecaoVazia()
    {
        // Arrange
        contexto.Necessidades.RemoveRange(contexto.Necessidades);
        contexto.SaveChanges();

        // Act
        var necessidades = servicoNecessidade.GetAll().ToList();

        // Assert
        Assert.IsNotNull(necessidades);
        Assert.AreEqual(0, necessidades.Count);
    }

    [TestMethod]
    public void Validar_QuandoDadosSaoValidos_DeveRetornarVerdadeiro()
    {
        // Arrange
        var necessidadeValida = new Necessidade
        {
            Nome = "Material escolar",
            Categoria = "Material escolar",
            Prioridade = "Baixa",
            IdInstituicao = 1
        };

        // Act
        var resultado = servicoNecessidade.Validar(necessidadeValida);

        // Assert
        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void Validar_QuandoPrioridadeInvalida_DeveRetornarFalso()
    {
        // Arrange
        var necessidadeInvalida = new Necessidade
        {
            Nome = "Material escolar",
            Categoria = "Material escolar",
            Prioridade = "Urgente",
            IdInstituicao = 1
        };

        // Act
        var resultado = servicoNecessidade.Validar(necessidadeInvalida);

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Validar_QuandoNomeVazio_DeveRetornarFalso()
    {
        // Arrange
        var necessidadeInvalida = new Necessidade
        {
            Nome = "   ",
            Categoria = "Alimentos",
            Prioridade = "Alta",
            IdInstituicao = 1
        };

        // Act
        var resultado = servicoNecessidade.Validar(necessidadeInvalida);

        // Assert
        Assert.IsFalse(resultado);
    }

    [TestMethod]
    public void Validar_QuandoInstituicaoNaoInformada_DeveRetornarFalso()
    {
        // Arrange
        var necessidadeInvalida = new Necessidade
        {
            Nome = "Cestas básicas",
            Categoria = "Alimentos",
            Prioridade = "Alta",
            IdInstituicao = 0
        };

        // Act
        var resultado = servicoNecessidade.Validar(necessidadeInvalida);

        // Assert
        Assert.IsFalse(resultado);
    }
}