using System.ComponentModel.DataAnnotations;

namespace OndeDoarWeb.Models
{
    public class InstituicaoViewModel : IValidatableObject
    {
        [Display(Name = "Código")]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Nome fantasia")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string NomeFantasia { get; set; } = null!;

        [Display(Name = "Razão social")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(40, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string RazaoSocial { get; set; } = null!;

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "O campo {0} deve ter 14 números, sem pontos, barra ou traço.")]
        public string Cnpj { get; set; } = null!;

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [RegularExpression(@"^\d{5}-\d{3}$", ErrorMessage = "O campo {0} deve estar no formato 00000-000.")]
        public string Cep { get; set; } = null!;

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(40, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Cidade { get; set; } = null!;

        [Display(Name = "Estado (UF)")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O campo {0} deve ter 2 letras, por exemplo SE.")]
        public string Estado { get; set; } = null!;

        [Display(Name = "Telefone")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "O campo {0} deve ter 10 ou 11 números, com DDD.")]
        public string? Telefone { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um {0} válido.")]
        [StringLength(40, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Hora de abertura")]
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public TimeOnly HoraAbertura { get; set; }

        [Display(Name = "Hora de fechamento")]
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public TimeOnly HoraFechamento { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HoraFechamento <= HoraAbertura)
            {
                yield return new ValidationResult(
                    "A hora de fechamento deve ser depois da hora de abertura.",
                    new[] { nameof(HoraFechamento) });
            }
        }
    }
}