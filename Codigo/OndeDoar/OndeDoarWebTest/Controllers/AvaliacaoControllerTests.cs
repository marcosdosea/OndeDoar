using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OndeDoarWeb.Controllers;
using OndeDoarWeb.Models;

namespace OndeDoarWebTest.Controllers.Tests;

[TestClass]
public class AvaliacaoControllerTests
{
    private Mock<IAvaliacaoService> avaliacaoServiceMock = null!;
    private AvaliacaoController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        avaliacaoServiceMock = new Mock<IAvaliacaoService>();

        avaliacaoServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterAvaliacoesDeTeste());

        avaliacaoServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterAvaliacaoAlvo());

        avaliacaoServiceMock
            .Setup(service => service.Get(999))
            .Returns((Avaliacao?)null);

        controller = new AvaliacaoController(avaliacaoServiceMock.Object);
    }

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComTodasAsAvaliacoes()
    {
        var resultado = controller.Index();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacoes = Assert.IsInstanceOfType<IEnumerable<AvaliacaoViewModel>>(viewResult.ViewData.Model);

        Assert.AreEqual(3, avaliacoes.Count());

        var primeiraAvaliacao = avaliacoes.First();
        Assert.AreEqual(1, primeiraAvaliacao.Id);
        Assert.AreEqual(5.0f, primeiraAvaliacao.Nota);

        avaliacaoServiceMock.Verify(service => service.GetAll(), Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemAvaliacoes_DeveRetornarViewComColecaoVazia()
    {
        avaliacaoServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Avaliacao>());

        var resultado = controller.Index();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacoes = Assert.IsInstanceOfType<IEnumerable<AvaliacaoViewModel>>(viewResult.ViewData.Model);

        Assert.IsFalse(avaliacoes.Any());
        avaliacaoServiceMock.Verify(service => service.GetAll(), Times.Once);
    }

    [TestMethod]
    public void Details_QuandoAvaliacaoExiste_DeveRetornarViewComAvaliacao()
    {
        var resultado = controller.Details(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacao = Assert.IsInstanceOfType<AvaliacaoViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1, avaliacao.Id);
        Assert.AreEqual(5.0f, avaliacao.Nota);

        avaliacaoServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Details_QuandoAvaliacaoNaoExiste_DeveRetornarViewComModeloVazio()
    {
        var resultado = controller.Details(999);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacao = Assert.IsInstanceOfType<AvaliacaoViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0, avaliacao.Id);
        Assert.AreEqual(0.0f, avaliacao.Nota);

        avaliacaoServiceMock.Verify(service => service.Get(999), Times.Once);
    }

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicial()
    {
        var resultado = controller.Create();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacao = Assert.IsInstanceOfType<AvaliacaoViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0, avaliacao.Id);
        Assert.IsFalse(avaliacao.Data == default);
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarAvaliacaoERedirecionarParaIndex()
    {
        var modelo = CriarModeloDeNovaAvaliacao();
        Avaliacao? avaliacaoCriada = null;

        avaliacaoServiceMock
            .Setup(service => service.Create(It.IsAny<Avaliacao>()))
            .Callback<Avaliacao>(avaliacao => avaliacaoCriada = avaliacao);

        var resultado = controller.Create(modelo);

        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(avaliacaoCriada);
        Assert.AreEqual(modelo.Id, avaliacaoCriada.Id);
        Assert.AreEqual(modelo.Nota, avaliacaoCriada.Nota);
        Assert.AreEqual(modelo.Comentario, avaliacaoCriada.Comentario);

        avaliacaoServiceMock.Verify(service => service.Create(It.IsAny<Avaliacao>()), Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarAvaliacao()
    {
        controller.ModelState.AddModelError(nameof(AvaliacaoViewModel.Nota), "A nota é obrigatória.");

        var resultado = controller.Create(CriarModeloDeNovaAvaliacao());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        avaliacaoServiceMock.Verify(service => service.Create(It.IsAny<Avaliacao>()), Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEAvaliacaoExiste_DeveRetornarViewComAvaliacao()
    {
        var resultado = controller.Edit(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacao = Assert.IsInstanceOfType<AvaliacaoViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1, avaliacao.Id);
        Assert.AreEqual(5.0f, avaliacao.Nota);

        avaliacaoServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEAvaliacaoNaoExiste_DeveRetornarViewComModeloVazio()
    {
        var resultado = controller.Edit(999);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacao = Assert.IsInstanceOfType<AvaliacaoViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0, avaliacao.Id);

        avaliacaoServiceMock.Verify(service => service.Get(999), Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarAvaliacaoERedirecionarParaIndex()
    {
        var modelo = CriarModeloDeAvaliacaoExistente();
        Avaliacao? avaliacaoEditada = null;

        avaliacaoServiceMock
            .Setup(service => service.Edit(It.IsAny<Avaliacao>()))
            .Callback<Avaliacao>(avaliacao => avaliacaoEditada = avaliacao);

        var resultado = controller.Edit(modelo);

        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(avaliacaoEditada);
        Assert.AreEqual(modelo.Id, avaliacaoEditada.Id);
        Assert.AreEqual(modelo.Nota, avaliacaoEditada.Nota);

        avaliacaoServiceMock.Verify(service => service.Edit(It.IsAny<Avaliacao>()), Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarAvaliacao()
    {
        controller.ModelState.AddModelError(nameof(AvaliacaoViewModel.Nota), "A nota é obrigatória.");

        var resultado = controller.Edit(CriarModeloDeAvaliacaoExistente());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        avaliacaoServiceMock.Verify(service => service.Edit(It.IsAny<Avaliacao>()), Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEAvaliacaoExiste_DeveRetornarViewComAvaliacao()
    {
        var resultado = controller.Delete(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacao = Assert.IsInstanceOfType<AvaliacaoViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1, avaliacao.Id);
        Assert.AreEqual(5.0f, avaliacao.Nota);

        avaliacaoServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEAvaliacaoNaoExiste_DeveRetornarViewComModeloVazio()
    {
        var resultado = controller.Delete(999);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var avaliacao = Assert.IsInstanceOfType<AvaliacaoViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0, avaliacao.Id);

        avaliacaoServiceMock.Verify(service => service.Get(999), Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirAvaliacaoERedirecionarParaIndex()
    {
        var modelo = CriarModeloDeAvaliacaoExistente();

        var resultado = controller.Delete(modelo);

        VerificarRedirecionamentoParaIndex(resultado);
        avaliacaoServiceMock.Verify(service => service.Delete(modelo.Id), Times.Once);
    }

    private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
    {
        var redirectResult = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
        Assert.IsNull(redirectResult.ControllerName);
        Assert.AreEqual(nameof(AvaliacaoController.Index), redirectResult.ActionName);
    }

    private static AvaliacaoViewModel CriarModeloDeNovaAvaliacao()
    {
        return new AvaliacaoViewModel
        {
            Id = 4,
            Nota = 4.5f,
            Comentario = "Excelente trabalho!",
            Data = new DateTime(2026, 11, 9),
            IdInstituicao = 10,
            IdDoador = 100
        };
    }

    private static AvaliacaoViewModel CriarModeloDeAvaliacaoExistente()
    {
        return new AvaliacaoViewModel
        {
            Id = 2,
            Nota = 3.5f,
            Comentario = "Bom",
            Data = new DateTime(2026, 10, 5),
            IdInstituicao = 11,
            IdDoador = 101
        };
    }

    private static Avaliacao ObterAvaliacaoAlvo()
    {
        return new Avaliacao
        {
            Id = 1,
            Nota = 5.0f,
            Comentario = "Ótimo!",
            Data = new DateTime(2026, 10, 1),
            IdInstituicao = 10,
            IdDoador = 100
        };
    }

    private static IEnumerable<Avaliacao> ObterAvaliacoesDeTeste()
    {
        return new List<Avaliacao>
        {
            new Avaliacao { Id = 1, Nota = 5.0f, Comentario = "Ótimo!", Data = new DateTime(2026, 10, 1), IdInstituicao = 10, IdDoador = 100 },
            new Avaliacao { Id = 2, Nota = 3.5f, Comentario = "Bom", Data = new DateTime(2026, 10, 5), IdInstituicao = 11, IdDoador = 101 },
            new Avaliacao { Id = 3, Nota = 4.0f, Comentario = "Muito bom", Data = new DateTime(2026, 10, 10), IdInstituicao = 12, IdDoador = 102 }
        };
    }
}