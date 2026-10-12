using System;
using System.Collections.Generic;

namespace MiProyectoApi.Models;

public partial class Entrenamiento
{
    public int IdEntrenamiento { get; set; }

    public int IdUsuario { get; set; }

    public int? IdRutina { get; set; }

    public DateTime Fecha { get; set; }

    public string? Nombre { get; set; }

    public virtual Rutina? IdRutinaNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Serie> Series { get; set; } = new List<Serie>();
}
