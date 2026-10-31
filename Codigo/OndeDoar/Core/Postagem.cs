using System;
using System.Collections.Generic;

namespace Core;

public partial class Postagem
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public DateTime Data { get; set; }

    public string Descricao { get; set; } = null!;

    public int InstituicaoId { get; set; }
}
