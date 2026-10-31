using System;
using System.Collections.Generic;

namespace Core;

public partial class Notificacao
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public int IdInstituicao { get; set; }
}
