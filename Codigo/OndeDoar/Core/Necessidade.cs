using System;
using System.Collections.Generic;

namespace Core;

public partial class Necessidade
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string? Descricao { get; set; }

    public string Prioridade { get; set; } = null!;

    public int IdInstituicao { get; set; }

    public byte[]? Status { get; set; }
}
