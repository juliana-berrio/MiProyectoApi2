using System.ComponentModel.DataAnnotations;

namespace MiProyectoApi.Dtos;

public record SerieInDto(
    [Range(1, int.MaxValue)] int IdEjercicio,
    [Range(1, 100)] int NumeroSerie,
    [Range(0, 1000)] decimal Peso,
    [Range(1, 1000)] int Repeticiones);

public record WorkoutCreateDto(
    int? IdRutina,
    [MaxLength(100)] string? Nombre,
    DateTime? Fecha,
    [Required, MinLength(1)] List<SerieInDto> Series);

public record SerieOutDto(
    int IdSerie,
    int IdEjercicio,
    string Ejercicio,
    int? NumeroSerie,
    decimal? Peso,
    int? Repeticiones);

public record WorkoutOutDto(
    int IdEntrenamiento,
    int? IdRutina,
    string? Nombre,
    DateTime? Fecha,
    decimal VolumenTotal,
    List<SerieOutDto> Series);

public record WorkoutResumenDto(
    int IdEntrenamiento,
    int? IdRutina,
    string? Nombre,
    DateTime? Fecha,
    int TotalSeries,
    decimal VolumenTotal);