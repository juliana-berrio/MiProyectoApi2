using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProyectoApi.Dtos;
using MiProyectoApi.Models;

namespace MiProyectoApi.Controllers;

[ApiController]
[Route("api/exercises")]
public class ExercisesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExercisesController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/exercises
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EjercicioDto>>> GetAll()
    {
        return await _context.Ejercicios
            .Select(e => new EjercicioDto
            {
                IdEjercicio = e.IdEjercicio,
                Nombre = e.Nombre,
                Descripcion = e.Descripcion,
                GrupoMuscular = e.GrupoMuscular
            })
            .ToListAsync();
    }

    // GET api/exercises/5
    [HttpGet("{id}")]
    public async Task<ActionResult<EjercicioDto>> GetById(int id)
    {
        var e = await _context.Ejercicios.FindAsync(id);
        if (e == null) return NotFound();

        return new EjercicioDto
        {
            IdEjercicio = e.IdEjercicio,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion,
            GrupoMuscular = e.GrupoMuscular
        };
    }

    // POST api/exercises
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EjercicioDto>> Create(EjercicioGuardarDto dto)
    {
        var e = new Ejercicio
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            GrupoMuscular = dto.GrupoMuscular
        };

        _context.Ejercicios.Add(e);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = e.IdEjercicio }, new EjercicioDto
        {
            IdEjercicio = e.IdEjercicio,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion,
            GrupoMuscular = e.GrupoMuscular
        });
    }

    // PUT api/exercises/5
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, EjercicioGuardarDto dto)
    {
        var e = await _context.Ejercicios.FindAsync(id);
        if (e == null) return NotFound();

        e.Nombre = dto.Nombre;
        e.Descripcion = dto.Descripcion;
        e.GrupoMuscular = dto.GrupoMuscular;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/exercises/5
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _context.Ejercicios.FindAsync(id);
        if (e == null) return NotFound();

        // No se puede borrar un ejercicio que ya usan rutinas o series
        bool enUso = await _context.RutinaEjercicios.AnyAsync(r => r.IdEjercicio == id)
                  || await _context.Series.AnyAsync(s => s.IdEjercicio == id);
        if (enUso)
            return Conflict("El ejercicio está en uso en rutinas o entrenamientos.");

        _context.Ejercicios.Remove(e);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}