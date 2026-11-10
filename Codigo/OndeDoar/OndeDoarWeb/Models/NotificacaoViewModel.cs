using System.ComponentModel.DataAnnotations;

namespace OndeDoarWeb.Models {
	public class NotificacaoViewModel {
		[Display(Name = "Código")]
		[Key]
		public int Id { get; set; }

		[Display(Name = "Título")]
		[Required(ErrorMessage = "O campo {0} é obrigatório.")]
		[StringLength(25, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
		public string Titulo { get; set; } = null!;

		[Display(Name = "Descrição")]
		[Required(ErrorMessage = "O campo {0} é obrigatório.")]
		[StringLength(250, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
		public string Descricao { get; set; } = null!;

		[Display(Name = "Instituição")]
		[Required(ErrorMessage = "O campo {0} é obrigatório.")]
		public int IdInstituicao { get; set; }
	}
}
