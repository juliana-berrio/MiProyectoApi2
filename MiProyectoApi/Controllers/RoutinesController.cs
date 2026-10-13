using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProyectoApi.Dtos;
using MiProyectoApi.Models;

namespace MiProyectoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/routines")]
public class RoutinesController(AppDbContext db) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Ejercicios de una rutina, ordenados
    private async Task<List<RutinaEjercicioOutDto>> EjerciciosDe(int idRutina)
    {
        var filas = await (from re in db.RutinaEjercicios
                           join e in db.Ejercicios on re.IdEjercicio equals e.IdEjercicio
                           where re.IdRutina == idRutina
                           orderby re.Orden
                           select new { e.IdEjercicio, e.Nombre, e.GrupoMuscular, re.Orden })
                          .ToListAsync();

        return filas.Select(f => new RutinaEjercicioOutDto(
            f.IdEjercicio, f.Nombre, f.GrupoMuscular, f.Orden)).ToList();
    }

    // Verifica que todos los ejercicios enviados existan en el catálogo
    private async Task<bool> EjerciciosValidos(List<RutinaEjercicioInDto> lista)
    {
        var ids = lista.Select(x => x.IdEjercicio).Distinct().ToList();
        var existentes = await db.Ejercicios.CountAsync(e => ids.Contains(e.IdEjercicio));
        return existentes == ids.Count;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rutinas = await db.Rutinas
            .Where(r => r.IdUsuario == UserId)
            .OrderByDescending(r => r.FechaCreacion)
            .ToListAsync();

        var resultado = new List<RutinaOutDto>();
        foreach (var r in rutinas)
        {
            resultado.Add(new RutinaOutDto(
                r.IdRutina, r.Nombre, r.Descripcion, r.FechaCreacion,
                await EjerciciosDe(r.IdRutina)));
        }
        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var r = await db.Rutinas
            .FirstOrDefaultAsync(x => x.IdRutina == id && x.IdUsuario == UserId);
        if (r is null) return NotFound();

        return Ok(new RutinaOutDto(
            r.IdRutina, r.Nombre, r.Descripcion, r.FechaCreacion,
            await EjerciciosDe(r.IdRutina)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(RutinaCreateDto dto)
    {
        var lista = dto.Ejercicios ?? new List<RutinaEjercicioInDto>();
        if (!await EjerciciosValidos(lista))
            return BadRequest(new { mensaje = "Alguno de los ejercicios no existe" });

        await using var tx = await db.Database.BeginTransactionAsync();

        var r = new Rutina
        {
            IdUsuario = UserId,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            FechaCreacion = DateTime.Now
        };
        db.Rutinas.Add(r);
        await db.SaveChangesAsync();   // aquí se genera r.IdRutina

        foreach (var e in lista)
        {
            db.RutinaEjercicios.Add(new RutinaEjercicio
            {
                IdRutina = r.IdRutina,
                IdEjercicio = e.IdEjercicio,
                Orden = e.Orden
            });
        }
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        var salida = new RutinaOutDto(
            r.IdRutina, r.Nombre, r.Descripcion, r.FechaCreacion,
            await EjerciciosDe(r.IdRutina));
        return CreatedAtAction(nameof(Get), new { id = r.IdRutina }, salida);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, RutinaCreateDto dto)
    {
        var r = await db.Rutinas
            .FirstOrDefaultAsync(x => x.IdRutina == id && x.IdUsuario == UserId);
        if (r is null) return NotFound();

        var lista = dto.Ejercicios ?? new List<RutinaEjercicioInDto>();
        if (!await EjerciciosValidos(lista))
            return BadRequest(new { mensaje = "Alguno de los ejercicios no existe" });

        await using var tx = await db.Database.BeginTransactionAsync();

        r.Nombre = dto.Nombre;
        r.Descripcion = dto.Descripcion;

        // Reemplaza la lista de ejercicios por la nueva
        var anteriores = db.RutinaEjercicios.Where(x => x.IdRutina == id);
        db.RutinaEjercicios.RemoveRange(anteriores);

        foreach (var e in lista)
        {
            db.RutinaEjercicios.Add(new RutinaEjercicio
            {
                IdRutina = id,
                IdEjercicio = e.IdEjercicio,
                Orden = e.Orden
            });
        }
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return Ok(new RutinaOutDto(
            r.IdRutina, r.Nombre, r.Descripcion, r.FechaCreacion,
            await EjerciciosDe(id)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await db.Rutinas
            .FirstOrDefaultAsync(x => x.IdRutina == id && x.IdUsuario == UserId);
        if (r is null) return NotFound();

        // Los entrenamientos ya registrados se conservan, pero sin rutina asociada
        await db.Entrenamientos
            .Where(e => e.IdRutina == id)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.IdRutina, (int?)null));

        db.Rutinas.Remove(r);   // RutinaEjercicio se borra en cascada
        await db.SaveChangesAsync();
        return NoContent();
}
}