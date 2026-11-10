using System;
using System.ComponentModel.DataAnnotations;

namespace OndeDoarWeb.Models
{
    public class PostagemViewModel
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório.")]
        public string Titulo { get; set; } = null!;

        public DateTime Data { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; } = null!;

        public int InstituicaoId { get; set; }
    }
}