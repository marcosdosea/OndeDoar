using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OndeDoarWeb.Controllers;
using OndeDoarWeb.Models;

namespace OndeDoarWebTest.Controllers.Tests;

[TestClass]
public class PostagemControllerTests
{
    private Mock<IPostagemService> postagemServiceMock = null!;
    private PostagemController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        postagemServiceMock = new Mock<IPostagemService>();

        postagemServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterPostagensDeTeste());

        postagemServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterPostagemAlvo());

        postagemServiceMock
            .Setup(service => service.Get(999))
            .Returns((Postagem?)null);

        controller = new PostagemController(postagemServiceMock.Object);
    }

    [TestMethod]
    public void Index_QuandoChamado_DeveRetornarViewComTodasAsPostagens()
    {
        var resultado = controller.Index();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagens = Assert.IsInstanceOfType<IEnumerable<PostagemViewModel>>(viewResult.ViewData.Model);

        Assert.AreEqual(3, postagens.Count());

        var primeiraPostagem = postagens.First();
        Assert.AreEqual(1u, primeiraPostagem.Id);
        Assert.AreEqual("Campanha de Alimentos", primeiraPostagem.Titulo);

        postagemServiceMock.Verify(service => service.GetAll(), Times.Once);
    }

    [TestMethod]
    public void Index_QuandoNaoExistemPostagens_DeveRetornarViewComColecaoVazia()
    {
        postagemServiceMock
            .Setup(service => service.GetAll())
            .Returns(Array.Empty<Postagem>());

        var resultado = controller.Index();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagens = Assert.IsInstanceOfType<IEnumerable<PostagemViewModel>>(viewResult.ViewData.Model);

        Assert.IsFalse(postagens.Any());
        postagemServiceMock.Verify(service => service.GetAll(), Times.Once);
    }

    [TestMethod]
    public void Details_QuandoPostagemExiste_DeveRetornarViewComPostagem()
    {
        var resultado = controller.Details(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagem = Assert.IsInstanceOfType<PostagemViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1u, postagem.Id);
        Assert.AreEqual("Campanha de Alimentos", postagem.Titulo);

        postagemServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Details_QuandoPostagemNaoExiste_DeveRetornarViewComModeloVazio()
    {
        var resultado = controller.Details(999);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagem = Assert.IsInstanceOfType<PostagemViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0u, postagem.Id);
        Assert.IsNull(postagem.Titulo);

        postagemServiceMock.Verify(service => service.Get(999), Times.Once);
    }

    [TestMethod]
    public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloInicial()
    {
        var resultado = controller.Create();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagem = Assert.IsInstanceOfType<PostagemViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0u, postagem.Id);
        Assert.IsFalse(postagem.Data == default);
    }

    [TestMethod]
    public void Create_QuandoModeloValido_DeveCriarPostagemERedirecionarParaIndex()
    {
        var modelo = CriarModeloDeNovaPostagem();
        Postagem? postagemCriada = null;

        postagemServiceMock
            .Setup(service => service.Create(It.IsAny<Postagem>()))
            .Callback<Postagem>(postagem => postagemCriada = postagem);

        var resultado = controller.Create(modelo);

        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(postagemCriada);
        Assert.AreEqual(modelo.Id, postagemCriada.Id);
        Assert.AreEqual(modelo.Titulo, postagemCriada.Titulo);
        Assert.AreEqual(modelo.Descricao, postagemCriada.Descricao);

        postagemServiceMock.Verify(service => service.Create(It.IsAny<Postagem>()), Times.Once);
    }

    [TestMethod]
    public void Create_QuandoModeloInvalido_NaoDeveCriarPostagem()
    {
        controller.ModelState.AddModelError(nameof(PostagemViewModel.Titulo), "O título é obrigatório.");

        var resultado = controller.Create(CriarModeloDeNovaPostagem());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        postagemServiceMock.Verify(service => service.Create(It.IsAny<Postagem>()), Times.Never);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEPostagemExiste_DeveRetornarViewComPostagem()
    {
        var resultado = controller.Edit(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagem = Assert.IsInstanceOfType<PostagemViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1u, postagem.Id);
        Assert.AreEqual("Campanha de Alimentos", postagem.Titulo);

        postagemServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoRequisicaoGetEPostagemNaoExiste_DeveRetornarViewComModeloVazio()
    {
        var resultado = controller.Edit(999);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagem = Assert.IsInstanceOfType<PostagemViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0u, postagem.Id);

        postagemServiceMock.Verify(service => service.Get(999), Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloValido_DeveAtualizarPostagemERedirecionarParaIndex()
    {
        var modelo = CriarModeloDePostagemExistente();
        Postagem? postagemEditada = null;

        postagemServiceMock
            .Setup(service => service.Edit(It.IsAny<Postagem>()))
            .Callback<Postagem>(postagem => postagemEditada = postagem);

        var resultado = controller.Edit(modelo);

        VerificarRedirecionamentoParaIndex(resultado);

        Assert.IsNotNull(postagemEditada);
        Assert.AreEqual(modelo.Id, postagemEditada.Id);
        Assert.AreEqual(modelo.Titulo, postagemEditada.Titulo);

        postagemServiceMock.Verify(service => service.Edit(It.IsAny<Postagem>()), Times.Once);
    }

    [TestMethod]
    public void Edit_QuandoModeloInvalido_NaoDeveAtualizarPostagem()
    {
        controller.ModelState.AddModelError(nameof(PostagemViewModel.Titulo), "O título é obrigatório.");

        var resultado = controller.Edit(CriarModeloDePostagemExistente());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        postagemServiceMock.Verify(service => service.Edit(It.IsAny<Postagem>()), Times.Never);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEPostagemExiste_DeveRetornarViewComPostagem()
    {
        var resultado = controller.Delete(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagem = Assert.IsInstanceOfType<PostagemViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(1u, postagem.Id);
        Assert.AreEqual("Campanha de Alimentos", postagem.Titulo);

        postagemServiceMock.Verify(service => service.Get(1), Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoGetEPostagemNaoExiste_DeveRetornarViewComModeloVazio()
    {
        var resultado = controller.Delete(999);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var postagem = Assert.IsInstanceOfType<PostagemViewModel>(viewResult.ViewData.Model);

        Assert.AreEqual(0u, postagem.Id);

        postagemServiceMock.Verify(service => service.Get(999), Times.Once);
    }

    [TestMethod]
    public void Delete_QuandoRequisicaoPost_DeveExcluirPostagemERedirecionarParaIndex()
    {
        var modelo = CriarModeloDePostagemExistente();

        var resultado = controller.Delete(modelo);

        VerificarRedirecionamentoParaIndex(resultado);
        postagemServiceMock.Verify(service => service.Delete(modelo.Id), Times.Once);
    }

    private static void VerificarRedirecionamentoParaIndex(ActionResult resultado)
    {
        var redirectResult = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
        Assert.IsNull(redirectResult.ControllerName);
        Assert.AreEqual(nameof(PostagemController.Index), redirectResult.ActionName);
    }

    private static PostagemViewModel CriarModeloDeNovaPostagem()
    {
        return new PostagemViewModel
        {
            Id = 4,
            Titulo = "Nova Campanha",
            Descricao = "Descrição da nova campanha",
            Data = new DateTime(2026, 11, 9),
            InstituicaoId = 2
        };
    }

    private static PostagemViewModel CriarModeloDePostagemExistente()
    {
        return new PostagemViewModel
        {
            Id = 2,
            Titulo = "Campanha Editada",
            Descricao = "Descrição atualizada",
            Data = new DateTime(2026, 10, 5),
            InstituicaoId = 1
        };
    }

    private static Postagem ObterPostagemAlvo()
    {
        return new Postagem
        {
            Id = 1,
            Titulo = "Campanha de Alimentos",
            Descricao = "Arrecadação de alimentos perecíveis",
            Data = new DateTime(2026, 10, 1),
            InstituicaoId = 1
        };
    }

    private static IEnumerable<Postagem> ObterPostagensDeTeste()
    {
        return new List<Postagem>
        {
            new Postagem { Id = 1, Titulo = "Campanha de Alimentos", Descricao = "Arrecadação de alimentos perecíveis", Data = new DateTime(2026, 10, 1), InstituicaoId = 1 },
            new Postagem { Id = 2, Titulo = "Campanha de Roupas", Descricao = "Arrecadação de agasalhos", Data = new DateTime(2026, 10, 5), InstituicaoId = 1 },
            new Postagem { Id = 3, Titulo = "Doação de Livros", Descricao = "Arrecadação de material escolar", Data = new DateTime(2026, 10, 10), InstituicaoId = 2 }
        };
    }
}