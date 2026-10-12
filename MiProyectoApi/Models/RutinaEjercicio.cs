using System;
using System.Collections.Generic;

namespace MiProyectoApi.Models;

public partial class RutinaEjercicio
{
    public int IdRutinaEjercicio { get; set; }

    public int IdRutina { get; set; }

    public int IdEjercicio { get; set; }

    public int Orden { get; set; }

    public virtual Ejercicio IdEjercicioNavigation { get; set; } = null!;

    public virtual Rutina IdRutinaNavigation { get; set; } = null!;
}
