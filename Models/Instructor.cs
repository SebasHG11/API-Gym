using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public ICollection<Clase> Clases { get; set; } = new List<Clase>();
    }
}