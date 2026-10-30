using System;
using System.Collections.Generic;

namespace Core;

public partial class Instituicaodoador
{
    public uint IdInstituicao { get; set; }

    public uint IdDoador { get; set; }

    public virtual Doador IdDoadorNavigation { get; set; } = null!;
}
