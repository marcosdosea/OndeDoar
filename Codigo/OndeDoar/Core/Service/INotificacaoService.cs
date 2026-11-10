using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service {
	public interface INotificacaoService {
		int Create(Notificacao notificacao);

		void Delete(int id);

		Notificacao? Get(int id);

		IEnumerable<Notificacao> GetAll();
	}
}