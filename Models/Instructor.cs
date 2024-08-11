using System.ComponentModel.DataAnnotations;

namespace ApiGym.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Especialidad { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<Clase> Clases { get; set; } = new List<Clase>();
    }
}