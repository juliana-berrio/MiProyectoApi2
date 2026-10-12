using System;
using System.Collections.Generic;

namespace MiProyectoApi.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<Entrenamiento> Entrenamientos { get; set; } = new List<Entrenamiento>();

    public virtual ICollection<Rutina> Rutinas { get; set; } = new List<Rutina>();
}
