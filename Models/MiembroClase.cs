namespace ApiGym.Models
{
    public class MiembroClase
    {
        public int MiembroId { get; set; }
        public Miembro Miembro { get; set; }
        public int ClaseId { get; set; }
        public Clase Clase { get; set; }
    }
}