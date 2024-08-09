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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>{
                entity.ToTable("Usuario");
                entity.HasKey(u => u.Id);
            });

            modelBuilder.Entity<Miembro>(entity =>{
                entity.ToTable("Miembro");
                entity.HasKey(m => m.Id);
                entity.HasOne(m => m.Usuario)
                .WithOne(u => u.Miembro)
                .HasForeignKey<Miembro>(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
                entity.Property(m => m.Nombre).HasMaxLength(30);
            });

            modelBuilder.Entity<Instructor>(entity =>{
                entity.ToTable("Instructor");
                entity.HasKey(i => i.Id);
                entity.HasOne(i => i.Usuario)
                .WithOne(u => u.Instructor)
                .HasForeignKey<Instructor>(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}