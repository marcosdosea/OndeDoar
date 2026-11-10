using Core;
using Core.Service;
using Microsoft.EntityFrameworkCore;

namespace Service {
	public class NotificacaoService : INotificacaoService {
		private readonly OndeDoarContext context;

		public NotificacaoService(OndeDoarContext context) {
			this.context = context;
		}

        /// <summary>
        ///		Cria uma nova notificação no banco de dados, retornando o id da notificação criada.
        /// </summary>
        ///		<param name="notificacao">
        ///			A notificação a ser criada.
        ///		</param>
        /// <returns>
        ///		Id da notificação criada.
        /// </returns>
        public int Create(Notificacao notificacao) {
			context.Add(notificacao);
			context.SaveChanges();

			return notificacao.Id;
		}

		/// <summary>
		///		Exclui uma notificação do banco de dados relativa a id fornecida.
		///		Nada é feito no caso que a notificação não existe.
		/// </summary>
		/// <param name="id">
		///		Id da notificação a ser deletada.
		/// </param>
		public void Delete(int id) {
			var notificacao = this.Get(id);

			if (notificacao == null) return;

			context.Remove(notificacao);
			context.SaveChanges();
		}

		/// <summary>
		///		Recupera uma notificação pelo id.
		/// </summary>
		///		<param name="id">
		///			Id da notificação.
		///		</param>
		/// <returns>
		///		Entidade Notificação
		/// </returns>
		public Notificacao? Get(int id) {
			return context.Notificacaos.Find(id);
		}

		/// <summary>
		///		Recupera todas as notificações do banco de dados.
		/// </summary>
		/// <returns>
		///		Uma Lista de Notificações.
		/// </returns>
		public IEnumerable<Notificacao> GetAll() {
			return context.Notificacaos.AsNoTracking();
		}
	}
}