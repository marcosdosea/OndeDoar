using System;
using System.Collections.Generic;
using System.Text;

namespace Core;

public partial class Instituicao
{
    public int Id { get; set; }
    public string NomeFantasia { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string Cep { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string? Telefone { get; set; }
    public string Email { get; set; } = null!;
    public string RazaoSocial { get; set; } = null!;
    public TimeOnly HoraAbertura { get; set; }
    public TimeOnly HoraFechamento { get; set; }
}