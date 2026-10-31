using System;
using System.Collections.Generic;

namespace Core;

public partial class Instituicaodoador
{
    public int IdInstituicao { get; set; }

    public int IdDoador { get; set; }

    public virtual Doador IdDoadorNavigation { get; set; } = null!;
}
