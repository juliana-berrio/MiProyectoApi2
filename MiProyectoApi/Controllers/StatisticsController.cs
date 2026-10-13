using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProyectoApi.Dtos;
using MiProyectoApi.Models;
using System.Security.Claims;

namespace MiProyectoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/statistics")]
public class StatisticsController(AppDbContext db) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private record FilaSerie(
        int IdEntrenamiento, DateTime? Fecha, int IdEjercicio,
        string Nombre, string GrupoMuscular, decimal? Peso, int? Rep);

    // Todas las series del usuario autenticado (consultas simples, sin join)
    private async Task<List<FilaSerie>> SeriesDelUsuario(int? idEjercicio = null)
    {
        // 1) Entrenamientos del usuario
        var entrenos = await db.Entrenamientos
            .Where(e => e.IdUsuario == UserId)
            .Select(e => new { e.IdEntrenamiento, Fecha = (DateTime?)e.Fecha })
            .ToListAsync();

        var idsEntreno = entrenos.Select(e => e.IdEntrenamiento).ToList();
        var fechas = entrenos.ToDictionary(e => e.IdEntrenamiento, e => e.Fecha);

        // 2) Series de esos entrenamientos
        var consulta = db.Series.Where(s => idsEntreno.Contains(s.IdEntrenamiento));
        if (idEjercicio is not null)
            consulta = consulta.Where(s => s.IdEjercicio == idEjercicio);

        var series = await consulta
            .Select(s => new
            {
                s.IdEntrenamiento,
                s.IdEjercicio,
                Peso = (decimal?)s.Peso,
                Rep = (int?)s.Repeticiones
            })
            .ToListAsync();

        // 3) Catálogo de ejercicios
        var ejercicios = await db.Ejercicios.ToDictionaryAsync(e => e.IdEjercicio);

        return series.Select(s => new FilaSerie(
            s.IdEntrenamiento,
            fechas[s.IdEntrenamiento],
            s.IdEjercicio,
            ejercicios[s.IdEjercicio].Nombre,
            ejercicios[s.IdEjercicio].GrupoMuscular,
            s.Peso,
            s.Rep)).ToList();
    }

    // Progreso general del usuario
    [HttpGet("progress")]
    public async Task<IActionResult> Progress()
    {
        var datos = await SeriesDelUsuario();

        var totalEntrenos = await db.Entrenamientos.CountAsync(e => e.IdUsuario == UserId);
        var ultimo = await db.Entrenamientos
            .Where(e => e.IdUsuario == UserId)
            .MaxAsync(e => (DateTime?)e.Fecha);

        var porEntreno = datos
            .GroupBy(d => new { d.IdEntrenamiento, d.Fecha })
            .OrderBy(g => g.Key.Fecha)
            .Select(g => new VolumenEntrenamientoDto(
                g.Key.IdEntrenamiento, g.Key.Fecha,
                g.Sum(x => (x.Peso ?? 0) * (x.Rep ?? 0))))
            .ToList();

        var masUsados = datos
            .GroupBy(d => new { d.IdEjercicio, d.Nombre })
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new EjercicioFrecuenteDto(g.Key.IdEjercicio, g.Key.Nombre, g.Count()))
            .ToList();

        return Ok(new ProgresoGeneralDto(
            totalEntrenos,
            datos.Count,
            datos.Sum(x => (x.Peso ?? 0) * (x.Rep ?? 0)),
            datos.Count == 0 ? 0 : datos.Max(x => x.Peso ?? 0),
            ultimo,
            porEntreno,
            masUsados));
    }

    // Progreso de un ejercicio específico
    [HttpGet("exercise/{id}")]
    public async Task<IActionResult> ExerciseProgress(int id)
    {
        var ejercicio = await db.Ejercicios.FirstOrDefaultAsync(e => e.IdEjercicio == id);
        if (ejercicio is null) return NotFound();

        var datos = await SeriesDelUsuario(id);

        var evolucion = datos
            .GroupBy(d => new { d.IdEntrenamiento, d.Fecha })
            .OrderBy(g => g.Key.Fecha)
            .Select(g => new PuntoProgresoDto(
                g.Key.IdEntrenamiento,
                g.Key.Fecha,
                g.Max(x => x.Peso ?? 0),
                g.Sum(x => x.Rep ?? 0),
                g.Sum(x => (x.Peso ?? 0) * (x.Rep ?? 0))))
            .ToList();

        return Ok(new ProgresoEjercicioDto(
            ejercicio.IdEjercicio,
            ejercicio.Nombre,
            ejercicio.GrupoMuscular,
            datos.Count,
            datos.Count == 0 ? 0 : datos.Max(x => x.Peso ?? 0),
            datos.Count == 0 ? 0 : datos.Max(x => x.Rep ?? 0),
            datos.Sum(x => (x.Peso ?? 0) * (x.Rep ?? 0)),
            evolucion));
    }
}