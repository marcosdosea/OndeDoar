using System;
using System.Collections.Generic;

namespace Core;

public partial class Necessidade
{
    public uint Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string? Descricao { get; set; }

    public string Prioridade { get; set; } = null!;

    public uint IdInstituicao { get; set; }

    public byte[]? Status { get; set; }
}
