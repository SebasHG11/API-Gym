using System.ComponentModel.DataAnnotations;

namespace ApiGym.Models
{
    public class Clase
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public DateTime FechaHora { get; set; }
        public int instructorId { get; set; }
        public Instructor Instructor { get; set; }
        public ICollection<MiembroClase> MiembrosClase { get; set; }
    }
}