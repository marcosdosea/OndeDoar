using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class NecessidadeViewModel
    {
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome da necessidade é obrigatório")]
        [StringLength(40, ErrorMessage = "Nome da necessidade deve ter no máximo 40 caracteres")]
        public string Nome { get; set; } = null!;

        [Display(Name = "Categoria")]
        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(40, ErrorMessage = "Categoria deve ter no máximo 40 caracteres")]
        public string Categoria { get; set; } = null!;

        [Display(Name = "Descrição")]
        [StringLength(100, ErrorMessage = "Descrição deve ter no máximo 100 caracteres")]
        public string? Descricao { get; set; }

        [Display(Name = "Prioridade")]
        [Required(ErrorMessage = "Prioridade é obrigatória")]
        public string Prioridade { get; set; } = null!;

        public int IdInstituicao { get; set; }

        [Display(Name = "Necessidade ativa")]
        public bool Ativa { get; set; } = true;
    }
}