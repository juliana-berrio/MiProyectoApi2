using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProyectoApi.Dtos;
using MiProyectoApi.Models;

namespace MiProyectoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/workouts")]
public class WorkoutsController(AppDbContext db) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Historial del usuario, del más reciente al más antiguo
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var entrenos = await db.Entrenamientos
            .Where(e => e.IdUsuario == UserId)
            .OrderByDescending(e => e.Fecha)
            .Select(e => new { e.IdEntrenamiento, e.IdRutina, e.Nombre, Fecha = (DateTime?)e.Fecha })
            .ToListAsync();

        var ids = entrenos.Select(e => e.IdEntrenamiento).ToList();

        var series = await db.Series
            .Where(s => ids.Contains(s.IdEntrenamiento))
            .Select(s => new
            {
                s.IdEntrenamiento,
                Peso = (decimal?)s.Peso,
                Rep = (int?)s.Repeticiones
            })
            .ToListAsync();

        var resultado = entrenos.Select(e =>
        {
            var propias = series.Where(s => s.IdEntrenamiento == e.IdEntrenamiento).ToList();
            var volumen = propias.Sum(s => (s.Peso ?? 0) * (s.Rep ?? 0));
            return new WorkoutResumenDto(
                e.IdEntrenamiento, e.IdRutina, e.Nombre, e.Fecha, propias.Count, volumen);
        });

        return Ok(resultado);
    }

    // Detalle de un entrenamiento con todas sus series
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var e = await db.Entrenamientos
            .FirstOrDefaultAsync(x => x.IdEntrenamiento == id && x.IdUsuario == UserId);
        if (e is null) return NotFound();

        return Ok(await Armar(e));
    }

    // Registrar un entrenamiento con sus series
    [HttpPost]
    public async Task<IActionResult> Create(WorkoutCreateDto dto)
    {
        // Si indica rutina, debe ser suya
        if (dto.IdRutina is not null)
        {
            var esSuya = await db.Rutinas
                .AnyAsync(r => r.IdRutina == dto.IdRutina && r.IdUsuario == UserId);
            if (!esSuya)
                return BadRequest(new { mensaje = "La rutina no existe o no te pertenece" });
        }

        // Todos los ejercicios deben existir
        var idsEj = dto.Series.Select(s => s.IdEjercicio).Distinct().ToList();
        var existentes = await db.Ejercicios.CountAsync(x => idsEj.Contains(x.IdEjercicio));
        if (existentes != idsEj.Count)
            return BadRequest(new { mensaje = "Alguno de los ejercicios no existe" });

        var e = new Entrenamiento
        {
            IdUsuario = UserId,
            IdRutina = dto.IdRutina,
            Nombre = dto.Nombre,
            Fecha = dto.Fecha ?? DateTime.Now,
            Series = dto.Series.Select(s => new Serie
            {
                IdEjercicio = s.IdEjercicio,
                NumeroSerie = s.NumeroSerie,
                Peso = s.Peso,
                Repeticiones = s.Repeticiones
            }).ToList()
        };

        db.Entrenamientos.Add(e);
        await db.SaveChangesAsync();   // guarda entrenamiento y series en una sola operación

        return CreatedAtAction(nameof(Get), new { id = e.IdEntrenamiento }, await Armar(e));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await db.Entrenamientos
            .FirstOrDefaultAsync(x => x.IdEntrenamiento == id && x.IdUsuario == UserId);
        if (e is null) return NotFound();

        await db.Series.Where(s => s.IdEntrenamiento == id).ExecuteDeleteAsync();
        db.Entrenamientos.Remove(e);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<WorkoutOutDto> Armar(Entrenamiento e)
    {
        var series = await (from s in db.Series
                            join ej in db.Ejercicios on s.IdEjercicio equals ej.IdEjercicio
                            where s.IdEntrenamiento == e.IdEntrenamiento
                            orderby s.IdEjercicio, s.NumeroSerie
                            select new
                            {
                                s.IdSerie,
                                s.IdEjercicio,
                                ej.Nombre,
                                NumeroSerie = (int?)s.NumeroSerie,
                                Peso = (decimal?)s.Peso,
                                Rep = (int?)s.Repeticiones
                            }).ToListAsync();

        var volumen = series.Sum(s => (s.Peso ?? 0) * (s.Rep ?? 0));

        return new WorkoutOutDto(
            e.IdEntrenamiento, e.IdRutina, e.Nombre, e.Fecha, volumen,
            series.Select(s => new SerieOutDto(
                s.IdSerie, s.IdEjercicio, s.Nombre, s.NumeroSerie, s.Peso, s.Rep)).ToList());
    }
}