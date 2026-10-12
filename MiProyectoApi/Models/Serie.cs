using System;
using System.Collections.Generic;

namespace MiProyectoApi.Models;

public partial class Serie
{
    public int IdSerie { get; set; }

    public int IdEntrenamiento { get; set; }

    public int IdEjercicio { get; set; }

    public int NumeroSerie { get; set; }

    public decimal Peso { get; set; }

    public int Repeticiones { get; set; }

    public virtual Ejercicio IdEjercicioNavigation { get; set; } = null!;

    public virtual Entrenamiento IdEntrenamientoNavigation { get; set; } = null!;
}
