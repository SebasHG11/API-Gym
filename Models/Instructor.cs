namespace ApiGym.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Especialidad { get; set; }
        public Usuario Usuario { get; set; }
    }
}