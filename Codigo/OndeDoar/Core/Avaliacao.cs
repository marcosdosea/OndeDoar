using System;
using System.Collections.Generic;

namespace Core;

public partial class Avaliacao
{
    public int Id { get; set; }

    public float Nota { get; set; }

    public string? Comentario { get; set; }

    public DateTime Data { get; set; }

    public int IdInstituicao { get; set; }

    public int IdDoador { get; set; }

    public virtual Doador IdDoadorNavigation { get; set; } = null!;
}
