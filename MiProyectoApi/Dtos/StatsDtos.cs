namespace MiProyectoApi.Dtos;

public record VolumenEntrenamientoDto(int IdEntrenamiento, DateTime? Fecha, decimal Volumen);

public record EjercicioFrecuenteDto(int IdEjercicio, string Nombre, int TotalSeries);

public record ProgresoGeneralDto(
    int TotalEntrenamientos,
    int TotalSeries,
    decimal VolumenTotal,
    decimal PesoMaximo,
    DateTime? UltimoEntrenamiento,
    List<VolumenEntrenamientoDto> VolumenPorEntrenamiento,
    List<EjercicioFrecuenteDto> EjerciciosMasUsados);

public record PuntoProgresoDto(
    int IdEntrenamiento,
    DateTime? Fecha,
    decimal PesoMaximo,
    int RepeticionesTotales,
    decimal Volumen);

public record ProgresoEjercicioDto(
    int IdEjercicio,
    string Nombre,
    string GrupoMuscular,
    int TotalSeries,
    decimal PesoMaximo,
    int RepeticionesMaximas,
    decimal VolumenTotal,
    List<PuntoProgresoDto> Evolucion);