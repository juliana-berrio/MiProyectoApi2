using System.ComponentModel.DataAnnotations;

namespace MiProyectoApi.Dtos;

public record RegisterDto(
    [Required, MaxLength(100)] string Nombre,
    [Required, EmailAddress, MaxLength(150)] string Correo,
    [Required, MinLength(6)] string Password);

public record LoginDto(
    [Required] string Correo,
    [Required] string Password);