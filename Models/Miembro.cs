using System.ComponentModel.DataAnnotations;

namespace ApiGym.Models
{
    public class Miembro
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        [Required]
        public string Nombre { get; set; }
        public Usuario? Usuario { get; set; }
        public ICollection<MiembroClase> MiembroClases { get; set; } = new List<MiembroClase>();
    }
}