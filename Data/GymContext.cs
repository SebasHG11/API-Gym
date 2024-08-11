using ApiGym.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGym.Data
{
    public class GymContext : DbContext
    {
        public GymContext(DbContextOptions<GymContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Miembro> Miembros { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Clase> Clases { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>{
                entity.ToTable("Usuario");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.HasIndex(u => u.UserName).IsUnique();
            });

            modelBuilder.Entity<Miembro>(entity =>{
                entity.ToTable("Miembro");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.HasOne(m => m.Usuario)
                .WithOne(u => u.Miembro)
                .HasForeignKey<Miembro>(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
                entity.Property(m => m.Nombre).HasMaxLength(30);
            });

            modelBuilder.Entity<Instructor>(entity =>{
                entity.ToTable("Instructor");
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id).ValueGeneratedOnAdd();
                entity.HasOne(i => i.Usuario)
                .WithOne(u => u.Instructor)
                .HasForeignKey<Instructor>(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Clase>(entity =>{
                entity.ToTable("Clase");
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.Instructor)
                    .WithMany(i => i.Clases)
                    .HasForeignKey(c => c.instructorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MiembroClase>(entity => {
                entity.ToTable("MiembroClase");
                entity.HasKey(mc => new { mc.MiembroId, mc.ClaseId });
                entity.HasOne(mc => mc.Miembro)
                    .WithMany(m => m.MiembroClases)
                    .HasForeignKey(mc => mc.MiembroId)
                    .OnDelete(DeleteBehavior.NoAction);
                entity.HasOne(mc => mc.Clase)
                    .WithMany(c => c.MiembrosClase)
                    .HasForeignKey(mc => mc.ClaseId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}