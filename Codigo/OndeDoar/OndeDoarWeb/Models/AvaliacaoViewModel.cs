using System;
using System.ComponentModel.DataAnnotations;

namespace OndeDoarWeb.Models
{
    public class AvaliacaoViewModel
    {
        [Display(Name = "Código")]
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(0, 5, ErrorMessage = "A {0} deve estar entre {1} e {2}.")]
        public float Nota { get; set; }

        [Display(Name = "Comentário")]
        [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? Comentario { get; set; }

        [Display(Name = "Data da Avaliação")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public DateTime Data { get; set; }

        [Display(Name = "Instituição")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int IdInstituicao { get; set; }

        [Display(Name = "Doador")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int IdDoador { get; set; }
    }
}