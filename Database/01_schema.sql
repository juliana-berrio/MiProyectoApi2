IF DB_ID('MiProyectoDb') IS NULL
    CREATE DATABASE MiProyectoDb;
GO
USE MiProyectoDb;
GO

IF OBJECT_ID('dbo.Usuario', 'U') IS NULL
CREATE TABLE Usuario (
    IdUsuario      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre         NVARCHAR(100) NOT NULL,
    Correo         NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash   NVARCHAR(255) NOT NULL,
    FechaRegistro  DATETIME NOT NULL DEFAULT GETDATE()
);

IF OBJECT_ID('dbo.Ejercicio', 'U') IS NULL
CREATE TABLE Ejercicio (
    IdEjercicio    INT IDENTITY(1,1) PRIMARY KEY,
    Nombre         NVARCHAR(100) NOT NULL,
    Descripcion    NVARCHAR(500) NULL,
    GrupoMuscular  NVARCHAR(50)  NOT NULL
);

IF OBJECT_ID('dbo.Rutina', 'U') IS NULL
CREATE TABLE Rutina (
    IdRutina       INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario      INT NOT NULL,
    Nombre         NVARCHAR(100) NOT NULL,
    Descripcion    NVARCHAR(500) NULL,
    FechaCreacion  DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Rutina_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
);

IF OBJECT_ID('dbo.RutinaEjercicio', 'U') IS NULL
CREATE TABLE RutinaEjercicio (
    IdRutinaEjercicio INT IDENTITY(1,1) PRIMARY KEY,
    IdRutina          INT NOT NULL,
    IdEjercicio       INT NOT NULL,
    Orden             INT NOT NULL,
    CONSTRAINT FK_RE_Rutina    FOREIGN KEY (IdRutina)    REFERENCES Rutina(IdRutina) ON DELETE CASCADE,
    CONSTRAINT FK_RE_Ejercicio FOREIGN KEY (IdEjercicio) REFERENCES Ejercicio(IdEjercicio)
);

IF OBJECT_ID('dbo.Entrenamiento', 'U') IS NULL
CREATE TABLE Entrenamiento (
    IdEntrenamiento INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario       INT NOT NULL,
    IdRutina        INT NULL,
    Fecha           DATETIME NOT NULL DEFAULT GETDATE(),
    Nombre          NVARCHAR(100) NULL,
    CONSTRAINT FK_Ent_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario),
    CONSTRAINT FK_Ent_Rutina  FOREIGN KEY (IdRutina)  REFERENCES Rutina(IdRutina) ON DELETE SET NULL
);

IF OBJECT_ID('dbo.Serie', 'U') IS NULL
CREATE TABLE Serie (
    IdSerie         INT IDENTITY(1,1) PRIMARY KEY,
    IdEntrenamiento INT NOT NULL,
    IdEjercicio     INT NOT NULL,
    NumeroSerie     INT NOT NULL,
    Peso            DECIMAL(6,2) NOT NULL,
    Repeticiones    INT NOT NULL,
    CONSTRAINT FK_Serie_Ent FOREIGN KEY (IdEntrenamiento) REFERENCES Entrenamiento(IdEntrenamiento) ON DELETE CASCADE,
    CONSTRAINT FK_Serie_Eje FOREIGN KEY (IdEjercicio)     REFERENCES Ejercicio(IdEjercicio)
);
GO

IF NOT EXISTS (SELECT 1 FROM Ejercicio)
BEGIN
    INSERT INTO Ejercicio (Nombre, Descripcion, GrupoMuscular) VALUES
    ('Press de banca', 'Empuje horizontal con barra', 'Pecho'),
    ('Sentadilla', 'Flexión de rodilla y cadera con barra', 'Pierna'),
    ('Peso muerto', 'Levantamiento desde el suelo', 'Espalda'),
    ('Dominadas', 'Tracción vertical con peso corporal', 'Espalda'),
    ('Press militar', 'Empuje vertical con barra', 'Hombro');
END
GO