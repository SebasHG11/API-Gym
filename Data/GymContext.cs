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
    }
}