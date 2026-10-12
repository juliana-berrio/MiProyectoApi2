using System;
using System.Collections.Generic;

namespace MiProyectoApi.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Correo { get; set; }
}
