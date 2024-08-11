using System.ComponentModel.DataAnnotations;

namespace ApiGym.Models
{
    public class CrearClaseDTO
    {
        public string Nombre { get; set; }
        public DateTime FechaHora { get; set; }
        public int? instructorId { get; set; }
    }
}