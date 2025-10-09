using Entidades.Entities;
using Microsoft.EntityFrameworkCore;

namespace Datos.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alumno> Alumno { get; set; }
        public DbSet<Asistencia> Asistencia { get; set; }
        public DbSet<Dia> Dia { get; set; }
        public DbSet<DiaHorario> DiaHorario { get; set; }
        public DbSet<DiaHorarioMateria> DiaHorarioMateria { get; set; }
        public DbSet<Examen> Examen { get; set; }
        public DbSet<Horario> Horario { get; set; }
        public DbSet<Inscripcion> Inscripcion { get; set; }
        public DbSet<Materia> Materia { get; set; }
        public DbSet<NotaAlumno> NotaAlumno { get; set; }
        public DbSet<Profesor> Profesor { get; set; }
        public DbSet<ProfesorMateria> ProfesorMateria { get; set; }
        public DbSet<RolUsuario> RolUsuario { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=SistemaGestionUniversitario;Integrated Security=True;TrustServerCertificate=true;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            modelBuilder.Entity<Alumno>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_ALUMNO");

                entity.HasMany(e => e.Materias)
                    .WithMany(e => e.Alumnos)
                    .UsingEntity<Inscripcion>
                    (
                       m => m.HasOne<Materia>().WithMany().HasForeignKey(e => e.IdMateria),
                       a => a.HasOne<Alumno>().WithMany().HasForeignKey(e => e.IdAlumno)
                    );

                entity.HasMany(e => e.Examenes)
                    .WithMany(e => e.Alumnos)
                    .UsingEntity<NotaAlumno>
                    (
                       e => e.HasOne<Examen>().WithMany().HasForeignKey(e => e.IdExamen),
                       a => a.HasOne<Alumno>().WithMany().HasForeignKey(e => e.IdAlumno)
                    );
            });
            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_ASISTENCIA");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
            });
            modelBuilder.Entity<Dia>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_DIA");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

                entity.HasMany(e => e.Horarios)
                    .WithMany(e => e.Dias)
                    .UsingEntity<DiaHorario>
                    (
                       h => h.HasOne<Horario>().WithMany().HasForeignKey(e => e.IdHorario),
                       d => d.HasOne<Dia>().WithMany().HasForeignKey(e => e.IdDia)
                    );
            });
            modelBuilder.Entity<DiaHorario>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_DIAHORARIO");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

                entity.HasMany(e => e.Materias)
                    .WithMany(e => e.DiaHorario)
                    .UsingEntity<DiaHorarioMateria>
                    (
                       m => m.HasOne<Materia>().WithMany().HasForeignKey(e => e.IdMateria),
                       d => d.HasOne<DiaHorario>().WithMany().HasForeignKey(e => e.IdDiaHorario)
                    );

                entity.HasMany(e => e.Examenes)
                    .WithOne(e => e.DiaHorario)
                    .HasForeignKey("IdDiaHorarioExamen")
                    .IsRequired();
            });
            modelBuilder.Entity<DiaHorarioMateria>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_DIAHORARIOMATERIA");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
            });
            modelBuilder.Entity<Examen>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_EXAMEN");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
            });
            modelBuilder.Entity<Horario>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_HORARIO");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
            });
            modelBuilder.Entity<Inscripcion>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_INSCRIPCION");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

                entity.HasMany(e => e.DiaHorarioMaterias)
                    .WithMany(e => e.Inscripciones)
                    .UsingEntity<Asistencia>
                    (
                       d => d.HasOne<DiaHorarioMateria>().WithMany().HasForeignKey(e => e.IdDiaHorarioMateria),
                       i => i.HasOne<Inscripcion>().WithMany().HasForeignKey(e => e.IdInscripcion)
                       .OnDelete(DeleteBehavior.Restrict)
                    );
            });
            modelBuilder.Entity<Materia>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_MATERIA");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

                entity.HasMany(e => e.Examenes)
                    .WithOne(e => e.Materia)
                    .HasForeignKey("IdMateria")
                    .IsRequired();
            });
            modelBuilder.Entity<NotaAlumno>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_NOTAALUMNO");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
            });
            modelBuilder.Entity<Profesor>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_PROFESOR");

                entity.HasMany(e => e.Materias)
                    .WithMany(e => e.Profesores)
                    .UsingEntity<ProfesorMateria>
                    (
                       m => m.HasOne<Materia>().WithMany().HasForeignKey(e => e.IdMateria),
                       p => p.HasOne<Profesor>().WithMany().HasForeignKey(e => e.IdProfesor)
                    );
            });
            modelBuilder.Entity<ProfesorMateria>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_PROFESORMATERIA");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");
            });
            modelBuilder.Entity<RolUsuario>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_ROLUSUARIO");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

                entity.HasMany(e => e.Usuarios)
                    .WithOne(e => e.RolUsuario)
                    .HasForeignKey("IdRolUsuario")
                    .IsRequired();
            });
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.ID)
                    .HasName("PK_ID_USUARIO");

                entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

                entity.HasMany(e => e.Profesores)
                    .WithOne(e => e.Usuario)
                    .HasForeignKey("IdProfesorUsuario")
                    .IsRequired(false);

                entity.HasMany(e => e.Alumnos)
                    .WithOne(e => e.Usuario)
                    .HasForeignKey("IdAlumnoUsuario")
                    .IsRequired(false);
            }); 
        }

        public override int SaveChanges()
        {
            this.DoCustomEntityPreparations();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.DoCustomEntityPreparations();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            this.DoCustomEntityPreparations();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void DoCustomEntityPreparations()
        {
            var modifiedEntitiesWithTrackDate = this
                .ChangeTracker.Entries()
                .Where(c => c.State == EntityState.Modified);
            foreach (var entityEntry in modifiedEntitiesWithTrackDate)
            {
                if (entityEntry.Properties.Any(c => c.Metadata.Name == "UpdatedDate"))
                {
                    entityEntry.Property("UpdatedDate").CurrentValue = DateTime.Now;
                }
            }
        }
    }
}