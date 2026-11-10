using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OndeDoarWeb.Controllers;
using OndeDoarWeb.Models;
using Service;

namespace OndeDoarWebTest.Controllers.Tests;

[TestClass]
public class InstituicaoControllerTests
{
    private Mock<IInstituicaoService> instituicaoServiceMock = null!;
    private InstituicaoController controller = null!;

    [TestInitialize]
    public void Inicializar()
    {
        instituicaoServiceMock = new Mock<IInstituicaoService>();

        instituicaoServiceMock
            .Setup(service => service.GetAll())
            .Returns(ObterInstituicoesDeTeste());

        instituicaoServiceMock
            .Setup(service => service.GetByNome("Solidária"))
            .Returns(ObterInstituicoesDeTeste().Where(i => i.Id == 2).ToList());

        instituicaoServiceMock
            .Setup(service => service.Get(1))
            .Returns(ObterInstituicoesDeTeste().First());

        instituicaoServiceMock
            .Setup(service => service.Get(999))
            .Returns((Instituicao?)null);

        controller = new InstituicaoController(instituicaoServiceMock.Object);
    }

    private static List<Instituicao> ObterInstituicoesDeTeste()
    {
        return
        [
            CriarInstituicao(1, "Lar Esperança", "lar@esperanca.org"),
            CriarInstituicao(2, "Casa Solidária", "casa@solidaria.org"),
            CriarInstituicao(3, "Abrigo Amigo", "abrigo@amigo.org")
        ];
    }

    private static Instituicao CriarInstituicao(int id, string nomeFantasia, string email)
    {
        return new Instituicao
        {
            Id = id,
            NomeFantasia = nomeFantasia,
            RazaoSocial = nomeFantasia + " LTDA",
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

    private static InstituicaoViewModel CriarViewModel(int id = 0)
    {
        return new InstituicaoViewModel
        {
            Id = id,
            NomeFantasia = "Nova Casa",
            RazaoSocial = "Nova Casa LTDA",
            Cnpj = "12345678000199",
            Cep = "49500-000",
            Cidade = "Itabaiana",
            Estado = "SE",
            Telefone = "79999999999",
            Email = "nova@casa.org",
            HoraAbertura = new TimeOnly(8, 0),
            HoraFechamento = new TimeOnly(17, 0)
        };
    }

    [TestMethod]
    public void Index_SemFiltro_DeveRetornarTodasAsInstituicoes()
    {
        var resultado = controller.Index(null);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var instituicoes = Assert.IsInstanceOfType<IEnumerable<InstituicaoViewModel>>(viewResult.ViewData.Model);

        Assert.HasCount(3, instituicoes);
        Assert.AreEqual("Lar Esperança", instituicoes.First().NomeFantasia);
        instituicaoServiceMock.Verify(service => service.GetAll(), Times.Once);
    }

    [TestMethod]
    public void Index_ComFiltro_DeveBuscarPorNome()
    {
        var resultado = controller.Index("Solidária");

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var instituicoes = Assert.IsInstanceOfType<IEnumerable<InstituicaoViewModel>>(viewResult.ViewData.Model);

        Assert.HasCount(1, instituicoes);
        Assert.AreEqual(2, instituicoes.First().Id);
        instituicaoServiceMock.Verify(service => service.GetByNome("Solidária"), Times.Once);
    }

    [TestMethod]
    public void Details_QuandoExiste_DeveRetornarViewComInstituicao()
    {
        var resultado = controller.Details(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var model = Assert.IsInstanceOfType<InstituicaoViewModel>(viewResult.ViewData.Model);
        Assert.AreEqual(1, model.Id);
        Assert.AreEqual("Lar Esperança", model.NomeFantasia);
    }

    [TestMethod]
    public void Details_QuandoNaoExiste_DeveRetornarNotFound()
    {
        var resultado = controller.Details(999);

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Create_Get_DeveRetornarViewComHorariosPadrao()
    {
        var resultado = controller.Create();

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsInstanceOfType<InstituicaoViewModel>(viewResult.ViewData.Model);
    }

    [TestMethod]
    public void Create_Post_QuandoValido_DeveChamarServiceERedirecionarParaIndex()
    {
        var resultado = controller.Create(CriarViewModel());

        var redirect = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
        Assert.AreEqual("Index", redirect.ActionName);
        instituicaoServiceMock.Verify(service => service.Create(It.IsAny<Instituicao>()), Times.Once);
    }

    [TestMethod]
    public void Create_Post_QuandoModelInvalido_DeveRetornarViewSemChamarService()
    {
        controller.ModelState.AddModelError("NomeFantasia", "Obrigatório");

        var resultado = controller.Create(CriarViewModel());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        instituicaoServiceMock.Verify(service => service.Create(It.IsAny<Instituicao>()), Times.Never);
    }

    [TestMethod]
    public void Create_Post_QuandoServiceLancaExcecao_DeveRetornarViewComErroNoModelState()
    {
        instituicaoServiceMock
            .Setup(service => service.Create(It.IsAny<Instituicao>()))
            .Throws(new ServiceException("O e-mail informado já está em uso por outra instituição."));

        var resultado = controller.Create(CriarViewModel());

        Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsFalse(controller.ModelState.IsValid);
    }

    [TestMethod]
    public void Edit_Get_QuandoExiste_DeveRetornarViewComInstituicao()
    {
        var resultado = controller.Edit(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var model = Assert.IsInstanceOfType<InstituicaoViewModel>(viewResult.ViewData.Model);
        Assert.AreEqual(1, model.Id);
    }

    [TestMethod]
    public void Edit_Get_QuandoNaoExiste_DeveRetornarNotFound()
    {
        var resultado = controller.Edit(999);

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Edit_Post_QuandoValido_DeveChamarServiceERedirecionarParaIndex()
    {
        var resultado = controller.Edit(CriarViewModel(1));

        var redirect = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
        Assert.AreEqual("Index", redirect.ActionName);
        instituicaoServiceMock.Verify(service => service.Edit(It.IsAny<Instituicao>()), Times.Once);
    }

    [TestMethod]
    public void Edit_Post_QuandoModelInvalido_DeveRetornarViewSemChamarService()
    {
        controller.ModelState.AddModelError("Email", "Inválido");

        var resultado = controller.Edit(CriarViewModel(1));

        Assert.IsInstanceOfType<ViewResult>(resultado);
        instituicaoServiceMock.Verify(service => service.Edit(It.IsAny<Instituicao>()), Times.Never);
    }

    [TestMethod]
    public void Edit_Post_QuandoServiceLancaExcecao_DeveRetornarViewComErroNoModelState()
    {
        instituicaoServiceMock
            .Setup(service => service.Edit(It.IsAny<Instituicao>()))
            .Throws(new ServiceException("O e-mail informado já está em uso por outra instituição."));

        var resultado = controller.Edit(CriarViewModel(1));

        Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsFalse(controller.ModelState.IsValid);
    }

    [TestMethod]
    public void Delete_Get_QuandoExiste_DeveRetornarViewComInstituicao()
    {
        var resultado = controller.Delete(1);

        var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
        var model = Assert.IsInstanceOfType<InstituicaoViewModel>(viewResult.ViewData.Model);
        Assert.AreEqual(1, model.Id);
    }

    [TestMethod]
    public void Delete_Get_QuandoNaoExiste_DeveRetornarNotFound()
    {
        var resultado = controller.Delete(999);

        Assert.IsInstanceOfType<NotFoundResult>(resultado);
    }

    [TestMethod]
    public void Delete_Post_DeveChamarServiceERedirecionarParaIndex()
    {
        var resultado = controller.Delete(CriarViewModel(1));

        var redirect = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);
        Assert.AreEqual("Index", redirect.ActionName);
        instituicaoServiceMock.Verify(service => service.Delete(1), Times.Once);
    }

    [TestMethod]
    public void Delete_Post_QuandoServiceLancaExcecao_DeveRetornarViewComErroNoModelState()
    {
        instituicaoServiceMock
            .Setup(service => service.Delete(1))
            .Throws(new ServiceException("Não foi possível excluir a instituição porque ela possui registros vinculados."));

        var resultado = controller.Delete(CriarViewModel(1));

        Assert.IsInstanceOfType<ViewResult>(resultado);
        Assert.IsFalse(controller.ModelState.IsValid);
    }
}