using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiProyectoApi.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ejercicio> Ejercicios { get; set; }

    public virtual DbSet<Entrenamiento> Entrenamientos { get; set; }

    public virtual DbSet<Rutina> Rutinas { get; set; }

    public virtual DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }

    public virtual DbSet<Serie> Series { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ejercicio>(entity =>
        {
            entity.HasKey(e => e.IdEjercicio).HasName("PK__Ejercici__F0E6D605EA7690FB");

            entity.ToTable("Ejercicio");

            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.GrupoMuscular).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Entrenamiento>(entity =>
        {
            entity.HasKey(e => e.IdEntrenamiento).HasName("PK__Entrenam__9FF8F99FBF969037");

            entity.ToTable("Entrenamiento");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.IdRutinaNavigation).WithMany(p => p.Entrenamientos)
                .HasForeignKey(d => d.IdRutina)
                .HasConstraintName("FK_Entr_Rutina");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Entrenamientos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_Entr_Usuario");
        });

        modelBuilder.Entity<Rutina>(entity =>
        {
            entity.HasKey(e => e.IdRutina).HasName("PK__Rutina__6E34CA3D3FA3AC66");

            entity.ToTable("Rutina");

            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Rutinas)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_Rutina_Usuario");
        });

        modelBuilder.Entity<RutinaEjercicio>(entity =>
        {
            entity.HasKey(e => e.IdRutinaEjercicio).HasName("PK__RutinaEj__40DD106A2FA80D19");

            entity.ToTable("RutinaEjercicio");

            entity.Property(e => e.Orden).HasDefaultValue(1);

            entity.HasOne(d => d.IdEjercicioNavigation).WithMany(p => p.RutinaEjercicios)
                .HasForeignKey(d => d.IdEjercicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RutEj_Ejercicio");

            entity.HasOne(d => d.IdRutinaNavigation).WithMany(p => p.RutinaEjercicios)
                .HasForeignKey(d => d.IdRutina)
                .HasConstraintName("FK_RutEj_Rutina");
        });

        modelBuilder.Entity<Serie>(entity =>
        {
            entity.HasKey(e => e.IdSerie).HasName("PK__Serie__13B9BEADC1CA391B");

            entity.ToTable("Serie");

            entity.Property(e => e.Peso).HasColumnType("decimal(6, 2)");

            entity.HasOne(d => d.IdEjercicioNavigation).WithMany(p => p.Series)
                .HasForeignKey(d => d.IdEjercicio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Serie_Ejercicio");

            entity.HasOne(d => d.IdEntrenamientoNavigation).WithMany(p => p.Series)
                .HasForeignKey(d => d.IdEntrenamiento)
                .HasConstraintName("FK_Serie_Entr");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF972BB3214D");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A198EA08358").IsUnique();

            entity.Property(e => e.Correo).HasMaxLength(150);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
