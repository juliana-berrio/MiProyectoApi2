using System.ComponentModel.DataAnnotations;

namespace MiProyectoApi.Dtos;

// Lo que la API devuelve
public class EjercicioDto
{
    public int IdEjercicio { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string GrupoMuscular { get; set; } = null!;
}

// Lo que la API recibe al crear o actualizar
public class EjercicioGuardarDto
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Required, StringLength(50)]
    public string GrupoMuscular { get; set; } = null!;
}