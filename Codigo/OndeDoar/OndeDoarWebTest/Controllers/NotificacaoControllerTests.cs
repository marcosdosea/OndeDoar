using Core;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OndeDoarWeb.Controllers;
using OndeDoarWeb.Models;

namespace OndeDoarWebTest.Controllers.Tests;

[TestClass]
public class NotificacaoControllerTests {
	private Mock<INotificacaoService> notificacaoServiceMock = null!;
	private NotificacaoController controller = null!;

	[TestInitialize]
	public void Inicializar() {
		notificacaoServiceMock = new Mock<INotificacaoService>();

		notificacaoServiceMock.Setup(service => service.GetAll()).Returns(ObterNotificacoesDeTeste());
		notificacaoServiceMock.Setup(service => service.Get(1)).Returns(ObterNotificacaoAlvo());
		notificacaoServiceMock.Setup(service => service.Get(999)).Returns((Notificacao?)null);

		controller = new NotificacaoController(notificacaoServiceMock.Object);
	}

	[TestMethod]
	public void Index_QuandoChamado_DeveRetornarViewComTodasAsNotificacoes() {
		var resultado = controller.Index();

		var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
		var notificacoes = Assert.IsInstanceOfType<IEnumerable<NotificacaoViewModel>>(viewResult.ViewData.Model).ToList();

		Assert.HasCount(3, notificacoes);
		Assert.AreEqual(1, notificacoes[0].Id);
		Assert.AreEqual("Campanha de Alimentos", notificacoes[0].Titulo);

		notificacaoServiceMock.Verify(service => service.GetAll(), Times.Once);
	}

	[TestMethod]
	public void Index_QuandoNaoExistemNotificacoes_DeveRetornarViewComColecaoVazia() {
		notificacaoServiceMock.Setup(service => service.GetAll()).Returns(Array.Empty<Notificacao>());

		var resultado = controller.Index();

		var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
		var notificacoes = Assert.IsInstanceOfType<IEnumerable<NotificacaoViewModel>>(viewResult.ViewData.Model).ToList();

		Assert.IsEmpty(notificacoes);

		notificacaoServiceMock.Verify(service => service.GetAll(), Times.Once);
	}

	[TestMethod]
	public void Details_QuandoNotificacaoExiste_DeveRetornarViewComNotificacao() {
		var resultado = controller.Details(1);

		var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
		var notificacao = Assert.IsInstanceOfType<NotificacaoViewModel>(viewResult.ViewData.Model);

		Assert.AreEqual(1, notificacao.Id);
		Assert.AreEqual("Campanha de Alimentos", notificacao.Titulo);
		Assert.AreEqual("Precisamos de cestas básicas para dezembro", notificacao.Descricao);
		Assert.AreEqual(1, notificacao.IdInstituicao);

		notificacaoServiceMock.Verify(service => service.Get(1), Times.Once);
	}

	[TestMethod]
	public void Details_QuandoNotificacaoNaoExiste_DeveRetornarViewComModeloVazio() {
		var resultado = controller.Details(999);

		var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
		var notificacao = Assert.IsInstanceOfType<NotificacaoViewModel>(viewResult.ViewData.Model);

		Assert.AreEqual(0, notificacao.Id);
		Assert.IsNull(notificacao.Titulo);

		notificacaoServiceMock.Verify(service => service.Get(999), Times.Once);
	}

	[TestMethod]
	public void Create_QuandoRequisicaoGet_DeveRetornarViewComModeloVazio() {
		var resultado = controller.Create();

		var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
		var notificacao = Assert.IsInstanceOfType<NotificacaoViewModel>(viewResult.ViewData.Model);

		Assert.AreEqual(0, notificacao.Id);
		Assert.IsNull(notificacao.Titulo);
	}

	[TestMethod]
	public void Create_QuandoModeloValido_DeveCriarNotificacaoERedirecionarParaIndex() {
		var modelo = CriarModeloDeNovaNotificacao();
		Notificacao? notificacaoCriada = null;

		notificacaoServiceMock
			.Setup(service => service.Create(It.IsAny<Notificacao>()))
			.Callback<Notificacao>(notificacao => notificacaoCriada = notificacao);

		var resultado = controller.Create(modelo);

		VerificarRedirecionamentoParaIndex(resultado);

		Assert.IsNotNull(notificacaoCriada);
		Assert.AreEqual(modelo.Id, notificacaoCriada.Id);
		Assert.AreEqual(modelo.Titulo, notificacaoCriada.Titulo);
		Assert.AreEqual(modelo.Descricao, notificacaoCriada.Descricao);
		Assert.AreEqual(modelo.IdInstituicao, notificacaoCriada.IdInstituicao);

		notificacaoServiceMock.Verify(service => service.Create(It.IsAny<Notificacao>()), Times.Once);
	}

	[TestMethod]
	public void Create_QuandoModeloInvalido_NaoDeveCriarNotificacao() {
		var modelo = CriarModeloDeNovaNotificacao();

		controller.ModelState.AddModelError(nameof(NotificacaoViewModel.Titulo), "O campo Título é obrigatório.");

		var resultado = controller.Create(modelo);

		var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);

		Assert.AreSame(modelo, viewResult.ViewData.Model);

		notificacaoServiceMock.Verify(service => service.Create(It.IsAny<Notificacao>()), Times.Never);
	}

	[TestMethod]
	public void Delete_QuandoRequisicaoGetENotificacaoExiste_DeveRetornarViewComNotificacao() {
		var resultado = controller.Delete(1);

		var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
		var notificacao = Assert.IsInstanceOfType<NotificacaoViewModel>(viewResult.ViewData.Model);

		Assert.AreEqual(1, notificacao.Id);
		Assert.AreEqual("Campanha de Alimentos", notificacao.Titulo);

		notificacaoServiceMock.Verify(service => service.Get(1), Times.Once);
	}

	[TestMethod]
	public void Delete_QuandoRequisicaoGetENotificacaoNaoExiste_DeveRetornarViewComModeloVazio() {
		var resultado = controller.Delete(999);

		var viewResult = Assert.IsInstanceOfType<ViewResult>(resultado);
		var notificacao = Assert.IsInstanceOfType<NotificacaoViewModel>(viewResult.ViewData.Model);

		Assert.AreEqual(0, notificacao.Id);

		notificacaoServiceMock.Verify(service => service.Get(999), Times.Once);
	}

	[TestMethod]
	public void Delete_QuandoRequisicaoPost_DeveExcluirNotificacaoERedirecionarParaIndex() {
		var modelo = CriarModeloDeNotificacaoExistente();

		var resultado = controller.Delete(modelo);

		VerificarRedirecionamentoParaIndex(resultado);

		notificacaoServiceMock.Verify(service => service.Delete(modelo.Id), Times.Once);
	}

	private static void VerificarRedirecionamentoParaIndex(ActionResult resultado) {
		var redirectResult = Assert.IsInstanceOfType<RedirectToActionResult>(resultado);

		Assert.IsNull(redirectResult.ControllerName);
		Assert.AreEqual(nameof(NotificacaoController.Index), redirectResult.ActionName);
	}

	private static NotificacaoViewModel CriarModeloDeNovaNotificacao() {
		return new NotificacaoViewModel {
			Id = 4,
			Titulo = "Doação de Brinquedos",
			Descricao = "Arrecadação para o Dia das Crianças",
			IdInstituicao = 3
		};
	}

	private static NotificacaoViewModel CriarModeloDeNotificacaoExistente() {
		return new NotificacaoViewModel {
			Id = 2,
			Titulo = "Campanha do Agasalho",
			Descricao = "Estamos arrecadando cobertores e casacos",
			IdInstituicao = 1
		};
	}

	private static Notificacao ObterNotificacaoAlvo() {
		return new Notificacao {
			Id = 1,
			Titulo = "Campanha de Alimentos",
			Descricao = "Precisamos de cestas básicas para dezembro",
			IdInstituicao = 1
		};
	}

	private static IEnumerable<Notificacao> ObterNotificacoesDeTeste() {
		return new List<Notificacao> {
			new Notificacao { Id = 1, Titulo = "Campanha de Alimentos", Descricao = "Precisamos de cestas básicas para dezembro", IdInstituicao = 1 },
			new Notificacao { Id = 2, Titulo = "Campanha do Agasalho", Descricao = "Estamos arrecadando cobertores e casacos", IdInstituicao = 1 },
			new Notificacao { Id = 3, Titulo = "Doação de Livros", Descricao = "Recebemos livros infantis até sexta-feira", IdInstituicao = 2 }
		};
	}
}
