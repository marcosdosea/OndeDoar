using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service.Tests;

[TestClass]
public class NotificacaoServiceTests {
	private OndeDoarContext contexto = null!;
	private INotificacaoService servicoNotificacao = null!;

	[TestInitialize]
	public void Inicializar() {
		var nomeBanco = $"OndeDoar_{Guid.NewGuid()}";

		var opcoes = new DbContextOptionsBuilder<OndeDoarContext>().UseInMemoryDatabase(nomeBanco).Options;

		contexto = new OndeDoarContext(opcoes);

		contexto.Notificacaos.AddRange(
			new Notificacao {
				Id = 1,
				Titulo = "Campanha de Alimentos",
				Descricao = "Precisamos de cestas básicas para dezembro",
				IdInstituicao = 1
			},
			new Notificacao {
				Id = 2,
				Titulo = "Campanha do Agasalho",
				Descricao = "Estamos arrecadando cobertores e casacos",
				IdInstituicao = 1
			},
			new Notificacao {
				Id = 3,
				Titulo = "Doação de Livros",
				Descricao = "Recebemos livros infantis até sexta-feira",
				IdInstituicao = 2
			}
		);

		contexto.SaveChanges();

		servicoNotificacao = new NotificacaoService(contexto);
	}

	[TestCleanup]
	public void Finalizar() {
		contexto.Dispose();
	}

	[TestMethod]
	public void Create_QuandoDadosSaoValidos_DeveRetornarIdDaNotificacaoCriada() {
		var notificacao = new Notificacao {
			Id = 4,
			Titulo = "Doação de Brinquedos",
			Descricao = "Arrecadação para o Dia das Crianças",
			IdInstituicao = 3
		};

		var idRetornado = servicoNotificacao.Create(notificacao);

		Assert.AreEqual(4, idRetornado);
	}

	[TestMethod]
	public void Create_QuandoDadosSaoValidos_DeveArmazenarNotificacao() {
		var notificacaoEsperada = new Notificacao {
			Id = 4,
			Titulo = "Doação de Brinquedos",
			Descricao = "Arrecadação para o Dia das Crianças",
			IdInstituicao = 3
		};

		servicoNotificacao.Create(notificacaoEsperada);

		var notificacaoCriada = servicoNotificacao.Get(4);

		Assert.IsNotNull(notificacaoCriada);
		Assert.AreEqual(notificacaoEsperada.Id, notificacaoCriada.Id);
		Assert.AreEqual(notificacaoEsperada.Titulo, notificacaoCriada.Titulo);
		Assert.AreEqual(notificacaoEsperada.Descricao, notificacaoCriada.Descricao);
		Assert.AreEqual(notificacaoEsperada.IdInstituicao, notificacaoCriada.IdInstituicao);
	}

	[TestMethod]
	public void Delete_QuandoNotificacaoExiste_DeveRemoverNotificacao() {
		servicoNotificacao.Delete(2);

		var notificacaoExcluida = servicoNotificacao.Get(2);
		var notificacoesRestantes = servicoNotificacao.GetAll().ToList();

		Assert.IsNull(notificacaoExcluida);
		Assert.HasCount(2, notificacoesRestantes);
		Assert.IsFalse(notificacoesRestantes.Any(notificacao => notificacao.Id == 2));
	}

	[TestMethod]
	public void Delete_QuandoNotificacaoNaoExiste_NaoDeveAlterarOsDados() {
		var notificacoesIniciais = servicoNotificacao.GetAll().ToList();

		servicoNotificacao.Delete(999);

		var notificacoesFinais = servicoNotificacao.GetAll().ToList();

		Assert.HasCount(notificacoesIniciais.Count, notificacoesFinais);

		CollectionAssert.AreEquivalent(
			notificacoesIniciais.Select(notificacao => notificacao.Id).ToList(),
			notificacoesFinais.Select(notificacao => notificacao.Id).ToList()
		);
	}

	[TestMethod]
	public void Get_QuandoIdExiste_DeveRetornarNotificacaoCorrespondente() {
		var notificacao = servicoNotificacao.Get(1);

		Assert.IsNotNull(notificacao);
		Assert.AreEqual(1, notificacao.Id);
		Assert.AreEqual("Campanha de Alimentos", notificacao.Titulo);
		Assert.AreEqual("Precisamos de cestas básicas para dezembro", notificacao.Descricao);
		Assert.AreEqual(1, notificacao.IdInstituicao);
	}

	[TestMethod]
	public void Get_QuandoIdNaoExiste_DeveRetornarNulo() {
		var notificacao = servicoNotificacao.Get(999);

		Assert.IsNull(notificacao);
	}

	[TestMethod]
	public void GetAll_QuandoExistemNotificacoes_DeveRetornarTodasAsNotificacoes() {
		var notificacoes = servicoNotificacao.GetAll().ToList();

		Assert.IsNotNull(notificacoes);
		Assert.HasCount(3, notificacoes);

		CollectionAssert.AreEquivalent(
			new int[] {1, 2, 3},
			notificacoes.Select(notificacao => notificacao.Id).ToList()
		);
	}

	[TestMethod]
	public void GetAll_QuandoNaoHaNotificacoes_DeveRetornarColecaoVazia() {
		contexto.Notificacaos.RemoveRange(contexto.Notificacaos);
		contexto.SaveChanges();

		var notificacoes = servicoNotificacao.GetAll().ToList();

		Assert.IsNotNull(notificacoes);
		Assert.IsEmpty(notificacoes);
	}
}