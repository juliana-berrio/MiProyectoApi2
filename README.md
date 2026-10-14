# FitTrack API

API REST para la aplicación móvil **FitTrack**: gestión de rutinas y seguimiento del progreso en entrenamientos de fuerza.

Proyecto de Programación de dispositivos móviles, Institución Universitaria Pascual Bravo.
Autores: Juliana Arboleda Berrio, Miguel Ángel Quintero Quiroz.

## Tecnologías

- ASP.NET Core Web API (.NET 10) con C#
- Entity Framework Core + SQL Server
- Autenticación JWT (Bearer)
- Documentación: Swagger UI y Scalar

## Requisitos

- .NET SDK 10
- SQL Server (Express o Developer) y SQL Server Management Studio (SSMS)
- Git

## Instalación y ejecución

1. Clonar el repositorio:
```bash
   git clone https://github.com/juliana-berrio/MiProyectoApi2.git
   cd MiProyectoApi2
```
2. Crear la base de datos: abrir `Database/01_schema.sql` en SSMS y ejecutarlo completo.
   El script crea la base `MiProyectoDb`, las 6 tablas y los ejercicios iniciales. Se puede ejecutar más de una vez sin duplicar datos.
3. Configurar la conexión (ver sección siguiente).
4. Ejecutar la API:
```bash
   cd MiProyectoApi
   dotnet run --launch-profile https
```
5. Abrir la documentación en el navegador:
   - Swagger: https://localhost:7098/swagger
   - Scalar: https://localhost:7098/scalar

## Configuración de la base de datos y JWT

Los valores por defecto están en `MiProyectoApi/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MiProyectoDb;Trusted_Connection=True;TrustServerCertificate=True;"
},
"Jwt": {
  "Key": "clave-de-ejemplo-cambiar-por-variable-de-entorno-32-caracteres-minimo",
  "Issuer": "FitTrack",
  "Audience": "FitTrackApp"
}
```

Para usar otro servidor o credenciales sin modificar el archivo, se pueden definir variables de entorno (doble guion bajo para los niveles):

```bash
# Windows (PowerShell)
$env:ConnectionStrings__DefaultConnection = "Server=MI_SERVIDOR;Database=MiProyectoDb;User Id=MI_USUARIO;Password=MI_CLAVE;TrustServerCertificate=True;"
$env:Jwt__Key = "una-clave-secreta-de-al-menos-32-caracteres"
```

La clave JWT debe tener al menos 32 caracteres. En un entorno real debe reemplazarse por una clave privada.

## Modelo de datos

Tablas: `Usuario`, `Ejercicio`, `Rutina`, `RutinaEjercicio`, `Entrenamiento` y `Serie`.
El esquema completo (DDL) está en `Database/01_schema.sql`.

## Autenticación

1. Registrar un usuario con `POST /api/auth/register`.
2. Iniciar sesión con `POST /api/auth/login`, que devuelve un `token`.
3. En Swagger, pulsar **Authorize** y pegar solo el token (sin la palabra Bearer).
4. En otros clientes, enviar el encabezado `Authorization: Bearer <token>`.

## Endpoints

| Método | Endpoint | Descripción | Requiere token |
|---|---|---|---|
| POST | /api/auth/register | Registrar usuario | No |
| POST | /api/auth/login | Iniciar sesión | No |
| GET | /api/users/profile | Consultar perfil | Sí |
| PUT | /api/users/profile | Actualizar perfil | Sí |
| GET | /api/exercises | Listar ejercicios | No |
| GET | /api/exercises/{id} | Consultar un ejercicio | No |
| POST | /api/exercises | Crear ejercicio | Sí |
| PUT | /api/exercises/{id} | Actualizar ejercicio | Sí |
| DELETE | /api/exercises/{id} | Eliminar ejercicio | Sí |
| GET | /api/routines | Listar rutinas del usuario | Sí |
| GET | /api/routines/{id} | Consultar una rutina | Sí |
| POST | /api/routines | Crear rutina con ejercicios | Sí |
| PUT | /api/routines/{id} | Actualizar rutina | Sí |
| DELETE | /api/routines/{id} | Eliminar rutina | Sí |
| GET | /api/workouts | Historial de entrenamientos | Sí |
| GET | /api/workouts/{id} | Detalle de un entrenamiento | Sí |
| POST | /api/workouts | Registrar entrenamiento con series | Sí |
| DELETE | /api/workouts/{id} | Eliminar entrenamiento | Sí |
| GET | /api/statistics/progress | Progreso general | Sí |
| GET | /api/statistics/exercise/{id} | Progreso de un ejercicio | Sí |

Cada usuario solo puede ver y modificar sus propias rutinas y entrenamientos.
