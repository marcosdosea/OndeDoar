using Core.Service;
using Microsoft.AspNetCore.Mvc;
using OndeDoarWeb.Mapper;
using OndeDoarWeb.Models;

namespace OndeDoarWeb.Controllers {
	public class NotificacaoController : Controller {
		private readonly INotificacaoService notificacaoService;

		public NotificacaoController(INotificacaoService notificacaoService) {
			this.notificacaoService = notificacaoService;
		}

		/// <summary>
		///		Exibe a lista com todas as notificações.
		/// </summary>
		/// <returns>
		///		View com a lista de notificações.
		/// </returns>
		public ActionResult Index() {
			var notificacoes = notificacaoService.GetAll();
			var notificacoesViewModel = NotificacaoMapper.ToViewModel(notificacoes);

			return View(notificacoesViewModel);
		}

		/// <summary>
		///		Exibe os detalhes de uma notificação.
		/// </summary>
		/// <param name="id">
		///		Id da notificação.
		/// </param>
		/// <returns>
		///		View com a notificação, ou com um modelo vazio no caso que a notificação não existe.
		/// </returns>
		public ActionResult Details(int id) {
			var notificacao = notificacaoService.Get(id);
			var notificacaoViewModel = notificacao != null ? NotificacaoMapper.ToViewModel(notificacao) : new NotificacaoViewModel();

			return View(notificacaoViewModel);
		}

		/// <summary>
		///		Exibe o formulário para criar uma nova notificação.
		/// </summary>
		/// <returns>
		///		View com um modelo vazio.
		/// </returns>
		public ActionResult Create() {
			return View(new NotificacaoViewModel());
		}

		/// <summary>
		///		Cria uma nova notificação a partir dos dados do formulário.
		/// </summary>
		/// <param name="notificacaoViewModel">
		///		Os dados da notificação a ser criada.
		/// </param>
		/// <returns>
		///		Redireciona para a lista de notificações, ou retorna o formulário no caso que os dados são inválidos.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(NotificacaoViewModel notificacaoViewModel) {
			if (!ModelState.IsValid) return View(notificacaoViewModel);

			var notificacao = NotificacaoMapper.ToEntity(notificacaoViewModel);
			notificacaoService.Create(notificacao);

			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		///		Exibe a confirmação de exclusão de uma notificação.
		/// </summary>
		/// <param name="id">
		///		Id da notificação.
		/// </param>
		/// <returns>
		///		View com a notificação, ou com um modelo vazio no caso que a notificação não existe.
		/// </returns>
		public ActionResult Delete(int id) {
			var notificacao = notificacaoService.Get(id);
			var notificacaoViewModel = notificacao != null ? NotificacaoMapper.ToViewModel(notificacao) : new NotificacaoViewModel();

			return View(notificacaoViewModel);
		}

		/// <summary>
		///		Exclui a notificação confirmada.
		/// </summary>
		/// <param name="notificacaoViewModel">
		///		A notificação a ser excluída.
		/// </param>
		/// <returns>
		///		Redireciona para a lista de notificações.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(NotificacaoViewModel notificacaoViewModel) {
			notificacaoService.Delete(notificacaoViewModel.Id);

			return RedirectToAction(nameof(Index));
		}
	}
}
