using System;
using System.Collections.Generic;

namespace MiProyectoApi.Models;

public partial class Ejercicio
{
    public int IdEjercicio { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string GrupoMuscular { get; set; } = null!;

    public virtual ICollection<RutinaEjercicio> RutinaEjercicios { get; set; } = new List<RutinaEjercicio>();

    public virtual ICollection<Serie> Series { get; set; } = new List<Serie>();
}
