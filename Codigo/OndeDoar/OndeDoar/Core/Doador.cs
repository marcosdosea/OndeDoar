using System;
using System.Collections.Generic;

namespace Core;

public partial class Doador
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public virtual ICollection<Avaliacao> Avaliacaos { get; set; } = new List<Avaliacao>();

    public virtual ICollection<Instituicaodoador> Instituicaodoadors { get; set; } = new List<Instituicaodoador>();
}
