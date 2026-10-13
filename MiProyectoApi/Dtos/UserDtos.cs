using System.ComponentModel.DataAnnotations;

namespace MiProyectoApi.Dtos;

public record PerfilDto(int IdUsuario, string Nombre, string Correo, DateTime FechaRegistro);

public record PerfilUpdateDto(
    [Required, MaxLength(100)] string Nombre,
    [Required, EmailAddress, MaxLength(150)] string Correo,
    [MinLength(6)] string? NuevaPassword);