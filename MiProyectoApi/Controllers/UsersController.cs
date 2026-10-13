using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProyectoApi.Dtos;
using MiProyectoApi.Models;

namespace MiProyectoApi.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController(AppDbContext db) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var u = await db.Usuarios.FindAsync(UserId);
        if (u is null) return NotFound();
        return Ok(new PerfilDto(u.IdUsuario, u.Nombre, u.Correo, u.FechaRegistro));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(PerfilUpdateDto dto)
    {
        var u = await db.Usuarios.FindAsync(UserId);
        if (u is null) return NotFound();

        var correoEnUso = await db.Usuarios
            .AnyAsync(x => x.Correo == dto.Correo && x.IdUsuario != UserId);
        if (correoEnUso)
            return Conflict(new { mensaje = "Ese correo ya está en uso" });

        u.Nombre = dto.Nombre;
        u.Correo = dto.Correo;
        if (!string.IsNullOrWhiteSpace(dto.NuevaPassword))
            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaPassword);

        await db.SaveChangesAsync();
        return Ok(new PerfilDto(u.IdUsuario, u.Nombre, u.Correo, u.FechaRegistro));
    }
}