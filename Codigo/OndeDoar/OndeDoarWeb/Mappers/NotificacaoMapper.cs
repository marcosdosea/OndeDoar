using Core;
using Mapster;
using OndeDoarWeb.Models;

namespace OndeDoarWeb.Mapper {
	public class NotificacaoMapper {
		/// <summary>
		///		Converte uma entidade Notificação em uma NotificacaoViewModel.
		/// </summary>
		/// <param name="notificacao">
		///		A notificação a ser convertida.
		/// </param>
		/// <returns>
		///		A NotificacaoViewModel correspondente.
		/// </returns>
		public static NotificacaoViewModel ToViewModel(Notificacao notificacao) {
			return notificacao.Adapt<NotificacaoViewModel>();
		}

		/// <summary>
		///		Converte uma NotificacaoViewModel em uma entidade Notificação.
		/// </summary>
		/// <param name="notificacaoViewModel">
		///		A NotificacaoViewModel a ser convertida.
		/// </param>
		/// <returns>
		///		A entidade Notificação correspondente.
		/// </returns>
		public static Notificacao ToEntity(NotificacaoViewModel notificacaoViewModel) {
			return notificacaoViewModel.Adapt<Notificacao>();
		}

		/// <summary>
		///		Converte uma lista de entidades Notificação em uma lista de NotificacaoViewModel.
		/// </summary>
		/// <param name="notificacoes">
		///		As notificações a serem convertidas.
		/// </param>
		/// <returns>
		///		Uma Lista de NotificacaoViewModel.
		/// </returns>
		public static IEnumerable<NotificacaoViewModel> ToViewModel(IEnumerable<Notificacao> notificacoes) {
			return notificacoes.Adapt<IEnumerable<NotificacaoViewModel>>();
		}

		/// <summary>
		///		Converte uma lista de NotificacaoViewModel em uma lista de entidades Notificação.
		/// </summary>
		/// <param name="notificacoesViewModel">
		///		As NotificacaoViewModel a serem convertidas.
		/// </param>
		/// <returns>
		///		Uma Lista de Notificações.
		/// </returns>
		public static IEnumerable<Notificacao> ToEntity(IEnumerable<NotificacaoViewModel> notificacoesViewModel) {
			return notificacoesViewModel.Adapt<IEnumerable<Notificacao>>();
		}
	}
}
