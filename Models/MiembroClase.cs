using System.Text.Json.Serialization;

namespace ApiGym.Models
{
    public class MiembroClase
    {
        public int MiembroId { get; set; }
        public Miembro Miembro { get; set; }
        public int ClaseId { get; set; }
        [JsonIgnore]
        public Clase Clase { get; set; }
    }
}