using System;
using System.Collections.Generic;

namespace Core;

public partial class Avaliacao
{
    public uint Id { get; set; }

    public float Nota { get; set; }

    public string? Comentario { get; set; }

    public DateTime Data { get; set; }

    public uint IdInstituicao { get; set; }

    public uint IdDoador { get; set; }

    public virtual Doador IdDoadorNavigation { get; set; } = null!;
}
