using System;
using System.Collections.Generic;

namespace MiProyectoApi.Models;

public partial class Rutina
{
    public int IdRutina { get; set; }

    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Entrenamiento> Entrenamientos { get; set; } = new List<Entrenamiento>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<RutinaEjercicio> RutinaEjercicios { get; set; } = new List<RutinaEjercicio>();
}
