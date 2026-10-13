using System.ComponentModel.DataAnnotations;

namespace MiProyectoApi.Dtos;

public record RutinaEjercicioInDto(int IdEjercicio, int Orden);

public record RutinaCreateDto(
    [Required, MaxLength(100)] string Nombre,
    [MaxLength(500)] string? Descripcion,
    List<RutinaEjercicioInDto>? Ejercicios);

public record RutinaEjercicioOutDto(int IdEjercicio, string Nombre, string GrupoMuscular, int Orden);

public record RutinaOutDto(
    int IdRutina,
    string Nombre,
    string? Descripcion,
    DateTime FechaCreacion,
    List<RutinaEjercicioOutDto> Ejercicios);
