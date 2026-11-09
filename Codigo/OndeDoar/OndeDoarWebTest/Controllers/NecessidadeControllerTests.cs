using AutoMapper;
using Core;
using Core.Service;
using Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Models;
using Moq;
using OndeDoarWeb.Controllers;
using Service;

namespace OndeDoarWebTest.Controllers.Tests;

[TestClass]
public class NecessidadeControllerTests
{
    // Mesmo valor da constante provisória do NecessidadeController.
    // Quando o login (CSU01) existir, este teste passa a simular a instituição autenticada.
    private const int IdInstituicaoAtual = 1;

    private Mock<INecessidadeService> necessidadeServiceMock = null!;
    private NecessidadeController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        necessidadeServiceMock = new Mock<INecessidadeService>();

        necessidadeServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterNecessidadesDeTeste());

        necessidadeServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterNecessidadeAlvo());

        // Necessidade que pertence a outra instituição.
        necessidadeServiceMock
            .Setup(service => service.Get(3))
            .Returns(ObterNecessidadesDeTeste().First(n => n.Id == 3));

        necessidadeServiceMock
            .Setup(service => service.Get(999))
            .Returns((Necessidade?)null);

        var configuracao = new MapperConfiguration(cfg => cfg.AddProfile<NecessidadeProfile>());
        var mapper = configuracao.CreateMapper();

        controller = new NecessidadeController(necessidadeServiceMock.Object, mapper);
    }

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarSomenteNecessidadesDaInstituicao()
    {
        var resultado = controller.Index();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var necessidades = Assert.IsInstanceOfType<List<NecessidadeModel>>(viewResult.ViewData.Model);

        Assert.AreEqual(2, necessidades.Count);
        CollectionAssert.AreEquivalent(
            new int[] { 1, 2 },
            necessidades.Select(n => n.Id).ToList());

        necessidadeServiceMock.Verify(service => service.GetAll(), Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemNecessidades_DeveRetornarViewComColecaoVazia()
    {
        necessidadeServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Necessidade>());

        var resultado = controller.Index();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var necessidades = Assert.IsInstanceOfType<List<NecessidadeModel>>(viewResult.ViewData.Model);

        Assert.AreEqual(0, necessidades.Count);
    }

    [TestMethod]
    public void Details_QuandoNecessidadeExiste_DeveRetornarViewComNecessidade()
    {
        var resultado = controller.Details(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var necessidade = Assert.IsInstanceOfType<NecessidadeModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1, necessidade.Id);
        Assert.AreEqual("Cestas básicas", necessidade.Nome);
        Assert.IsTrue(necessidade.Ativa);

        necessidadeServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Details_QuandoNecessidadeNaoExiste_DeveRetornarNotFound()
    {
        var resultado = controller.Details(999);

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Details_QuandoNecessidadeEDeOutraInstituicao_DeveRetornarNotFound()
    {
        var resultado = controller.Details(3);

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicial()
    {
        var resultado = controller.Create();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var necessidade = Assert.IsInstanceOfType<NecessidadeModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0, necessidade.Id);
        Assert.IsTrue(necessidade.Ativa);
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarNecessidadeERedirecionarParaIndex()
    {
        var modelo = CriarModeloDeNovaNecessidade();
        Necessidade? necessidadeCriada = null;

        necessidadeServiceMock
            .Setup(service => service.Create(It.IsAny<Necessidade>()))
            .Callback<Necessidade>(necessidade => necessidadeCriada = necessidade);

        var resultado = controller.Create(modelo);

        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(necessidadeCriada);
        Assert.AreEqual(modelo.Nome, necessidadeCriada.Nome);
        Assert.AreEqual(modelo.Categoria, necessidadeCriada.Categoria);
        Assert.AreEqual(modelo.Prioridade, necessidadeCriada.Prioridade);
        // O controller define a instituição, ignorando o que veio do formulário.
        Assert.AreEqual(IdInstituicaoAtual, necessidadeCriada.IdInstituicao);
        Assert.AreEqual((byte)1, necessidadeCriada.Status![0]);

        necessidadeServiceMock.Verify(service => service.Create(It.IsAny<Necessidade>()), Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarNecessidade()
    {
        controller.ModelState.AddModelError(nameof(NecessidadeModel.Nome), "Nome da necessidade é obrigatório");

        var resultado = controller.Create(CriarModeloDeNovaNecessidade());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        necessidadeServiceMock.Verify(service => service.Create(It.IsAny<Necessidade>()), Times.Never);
    }

    [TestMethod]
    public void Create_QuandoServiceLancaServiceException_DeveRetornarViewComErroNoModelState()
    {
        necessidadeServiceMock
            .Setup(service => service.Create(It.IsAny<Necessidade>()))
            .Throws(new ServiceException("Os dados da necessidade são inválidos.", new Exception()));

        var resultado = controller.Create(CriarModeloDeNovaNecessidade());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsFalse(controller.ModelState.IsValid);
        Assert.AreEqual(
            "Os dados da necessidade são inválidos.",
            controller.ModelState[string.Empty]!.Errors[0].ErrorMessage);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetENecessidadeExiste_DeveRetornarViewComNecessidade()
    {
        var resultado = controller.Edit(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var necessidade = Assert.IsInstanceOfType<NecessidadeModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1, necessidade.Id);
        Assert.AreEqual("Alta", necessidade.Prioridade);

        necessidadeServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetENecessidadeNaoExiste_DeveRetornarNotFound()
    {
        var resultado = controller.Edit(999);

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetENecessidadeEDeOutraInstituicao_DeveRetornarNotFound()
    {
        var resultado = controller.Edit(3);

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarNecessidadeERedirecionarParaIndex()
    {
        var modelo = CriarModeloDeNovaNecessidade();
        modelo.Id = 0;
        Necessidade? necessidadeEditada = null;

        necessidadeServiceMock
            .Setup(service => service.Edit(It.IsAny<Necessidade>()))
            .Callback<Necessidade>(necessidade => necessidadeEditada = necessidade);

        var resultado = controller.Edit(1, modelo);

        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(necessidadeEditada);
        // O Id vem da rota, e a instituição é definida pelo controller.
        Assert.AreEqual(1, necessidadeEditada.Id);
        Assert.AreEqual(IdInstituicaoAtual, necessidadeEditada.IdInstituicao);
        Assert.AreEqual(modelo.Nome, necessidadeEditada.Nome);

        necessidadeServiceMock.Verify(service => service.Edit(It.IsAny<Necessidade>()), Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarNecessidade()
    {
        controller.ModelState.AddModelError(nameof(NecessidadeModel.Nome), "Nome da necessidade é obrigatório");

        var resultado = controller.Edit(1, CriarModeloDeNovaNecessidade());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        necessidadeServiceMock.Verify(service => service.Edit(It.IsAny<Necessidade>()), Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoNecessidadeNaoExiste_DeveRetornarNotFoundENaoAtualizar()
    {
        var resultado = controller.Edit(999, CriarModeloDeNovaNecessidade());

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
        necessidadeServiceMock.Verify(service => service.Edit(It.IsAny<Necessidade>()), Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetENecessidadeExiste_DeveRetornarViewComNecessidade()
    {
        var resultado = controller.Delete(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var necessidade = Assert.IsInstanceOfType<NecessidadeModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1, necessidade.Id);
        necessidadeServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetENecessidadeNaoExiste_DeveRetornarNotFound()
    {
        var resultado = controller.Delete(999);

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirNecessidadeERedirecionarParaIndex()
    {
        var resultado = controller.Delete(1, new FormCollection(new Dictionary<string, StringValues>()));

        VerificarRedirecionamentoParaIndex(resultado);
        necessidadeServiceMock.Verify(service => service.Delete(1), Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPostENecessidadeEDeOutraInstituicao_DeveRetornarNotFoundENaoExcluir()
    {
        var resultado = controller.Delete(3, new FormCollection(new Dictionary<string, StringValues>()));

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
        necessidadeServiceMock.Verify(service => service.Delete(It.IsAny<int>()), Times.Never);
    }

    private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
    {
        var redirectResult = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
        Assert.IsNull(redirectResult.ControllerName);
        Assert.AreEqual(nameof(NecessidadeController.Index), redirectResult.ActionName);
    }

    private static NecessidadeModel CriarModeloDeNovaNecessidade()
    {
        return new NecessidadeModel
        {
            Id = 4,
            Nome = "Fraldas infantis",
            Categoria = "Saúde",
            Descricao = "Fraldas tamanho M",
            Prioridade = "Média",
            IdInstituicao = 99,
            Ativa = true
        };
    }

    private static Necessidade ObterNecessidadeAlvo()
    {
        return ObterNecessidadesDeTeste().First(n => n.Id == 1);
    }

    private static IEnumerable<Necessidade> ObterNecessidadesDeTeste()
    {
        return new List<Necessidade>
        {
            new Necessidade { Id = 1, Nome = "Cestas básicas", Categoria = "Alimentos", Descricao = "Alimentos não perecíveis", Prioridade = "Alta", IdInstituicao = 1, Status = new byte[] { 1 } },
            new Necessidade { Id = 2, Nome = "Roupas infantis", Categoria = "Roupas", Descricao = "Roupas de 4 a 12 anos", Prioridade = "Média", IdInstituicao = 1, Status = new byte[] { 1 } },
            new Necessidade { Id = 3, Nome = "Produtos de higiene", Categoria = "Higiene", Descricao = "Sabonetes e shampoos", Prioridade = "Baixa", IdInstituicao = 2, Status = new byte[] { 0 } }
        };
    }
}
