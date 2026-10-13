using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiProyectoApi.Dtos;
using MiProyectoApi.Models;
using MiProyectoApi.Services;

namespace MiProyectoApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, TokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await db.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
            return Conflict(new { mensaje = "El correo ya está registrado" });

        var u = new Usuario
        {
            Nombre = dto.Nombre,
            Correo = dto.Correo,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FechaRegistro = DateTime.Now
        };
        db.Usuarios.Add(u);
        await db.SaveChangesAsync();
        return Created("", new { u.IdUsuario, u.Nombre, u.Correo });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var u = await db.Usuarios.FirstOrDefaultAsync(x => x.Correo == dto.Correo);
        if (u is null || !BCrypt.Net.BCrypt.Verify(dto.Password, u.PasswordHash))
            return Unauthorized(new { mensaje = "Credenciales inválidas" });

        return Ok(new { token = tokens.Crear(u) });
    }
}
