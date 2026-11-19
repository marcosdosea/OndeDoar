using System.ComponentModel.DataAnnotations;

namespace OndeDoarWeb.Models
{
    public class VerificacaoViewModel
    {
        public int IdInstituicao { get; set; }

        [Display(Name = "Nome completo")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(60, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string NomeResponsavel { get; set; } = null!;

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "O campo {0} deve ter 11 números, sem pontos ou traço.")]
        public string Cpf { get; set; } = null!;

        [Display(Name = "Cargo ou função")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(40, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Cargo { get; set; } = null!;

        [Display(Name = "E-mail do responsável")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um {0} válido.")]
        public string EmailResponsavel { get; set; } = null!;

        public bool Confirmo { get; set; }
    }

    public class PerfilPublicoViewModel
    {
        public int IdInstituicao { get; set; }

        public string? Cidade { get; set; }

        public string? Estado { get; set; }

        [Display(Name = "Nome exibido")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string NomeExibido { get; set; } = null!;

        [Display(Name = "Sobre a instituição")]
        [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? Sobre { get; set; }

        [Display(Name = "Visível para doadores")]
        public bool Visivel { get; set; } = true;
    }
}