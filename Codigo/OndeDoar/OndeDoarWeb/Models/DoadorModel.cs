using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class DoadorModel
    {
        [Display(Name = "Código")]
        [Key]
        [Required(ErrorMessage = "Código do doador é obrigatório")]
        public int Id { get; set; }


        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email do doador é obrigatório")]
        [StringLength(40, ErrorMessage = "Email do doador deve ter no máximo 40 caracteres")]
        public string Email { get; set; }


        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "Telefone do doador é obrigatório")]
        [StringLength(11, ErrorMessage = "Telefone do doador deve ter no máximo 11 caracteres")]
        public string Telefone { get; set; }


        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome do doador é obrigatório")]
        [StringLength(40, ErrorMessage = "Nome do doador deve ter no máximo 40 caracteres")]
        public string Nome { get; set; }
    }
}
